using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LiquidPhysics : MonoBehaviour
{
    /* Max angle the liquid can move */
    private const int MaxMovementAngle = 45;
    
    /* Cached shader properties for fast access */
    private static readonly int PlaneNormal = Shader.PropertyToID("_PlaneNormal");
    private static readonly int PlanePoint = Shader.PropertyToID("_PlanePoint");
    
    /* The current material */
    private Material _liquidMaterial;
    
    /* Amount the water should move */
    private Vector2 _moveAmount;
    private Vector2 _moveAmountToAdd;
    
    /* The current time */
    private float _time = 0.5f;
    
    /* The height of the submesh which has this material*/
    private float _meshHeight;
    
    /* The last saved position */
    private Vector3 _lastPos;
    
    /* The last saved rotation */
    private Vector3 _lastRot;
    
    /* The last saved vertices */
    private Vector3[] _vertices;
    
    /* A vector that represents the lowest vertex of the transformed mesh */
    private Vector3 _lowestPoint;
    private Vector3 _newLowestPoint;
    
    [Tooltip("A float in the range [0, 1] that represents the volume the fluid should have")]
    [Range(0.0f, 1.0f)]
    public float VolumePercentage = 0.5f;

    [Tooltip("The max movement the liquid can make")]
    public float MaxMovement = 0.05f;

    [Tooltip("The speed of the waves generated when moving the liquid transform")]
    public float WaveSpeed = 1f;

    [Range(0.0f, 5.0f)]
    [Tooltip("The speed at which the liquid recovers from movements")]
    public float RecoverySpeed = 1f;

    void Start()
    {
        Initialize();
    }

    /* Gets the necessary components */
    private void Initialize()
    {
        var rend = GetComponent<Renderer>();
        var filter = GetComponent<MeshFilter>();
        var materials = rend.materials;
        
        /* Find the fluid material */
        for (var i = 0; i < materials.Length; ++i)
        {
            if (materials[i].shader.name != "Bytesized/Fluid") continue;
            /* Set the current material as the one we will use */
            _liquidMaterial = materials[i];
            /* Get vertices for the submesh */
            var vertices = filter.sharedMesh.vertices;
            var triangles = filter.sharedMesh.GetTriangles(i);
            _vertices = triangles.Select(T => vertices[T]).ToArray();
            var subMeshVertices = _vertices.Select(T => Vector3.Scale(transform.localScale, T)).ToArray();
            /* Find out the size of the submesh segment */
            var max = SupportPoint(subMeshVertices, Vector3.up).y;
            var min = SupportPoint(subMeshVertices, -Vector3.up).y;
            _meshHeight = Math.Max(
                Math.Max(
                    SupportPoint(subMeshVertices, Vector3.up).y - SupportPoint(subMeshVertices, -Vector3.up).y,
                    SupportPoint(subMeshVertices, Vector3.right).x - SupportPoint(subMeshVertices, -Vector3.right).x
                ),
                SupportPoint(subMeshVertices, Vector3.forward).z - SupportPoint(subMeshVertices, -Vector3.forward).z
            ); 
            break;
        }
        ForceUpdateLowestPoint();
    }

    private bool NeedsInitialization()
    {
        return _liquidMaterial == null;
    }

    /* Finds the closest vector to the provided direction from an array */
    private static Vector3 SupportPoint(Vector3[] Vectors, Vector3 Direction)
    {
        var highest = float.MinValue;
        var most = Vector3.zero;
        for (var i = 0; i < Vectors.Length; ++i)
        {
            /* Compare against every vector and get the one with the highest dot product */
            var dot = Vector3.Dot(Vectors[i], Direction);
            if (dot > highest)
            {
                highest = dot;
                most = Vectors[i];
            }
        }
        /* Return the nearest to the direction */
        return most;
    }
    
    private void Update()
    {
        if (NeedsInitialization())
        {
            Initialize();
        }

        /* Transform properties */
        var position = transform.position;
        var rotation = transform.rotation;

        UpdateRecovery();

        var finalNormal = CalculateLiquidNormal();
        SendPlaneInformation(finalNormal);
        UpdateLowestPointIfNecessary(rotation);
        CalculateLiquidVelocity(position, rotation);
        
        Debug.DrawLine(_lowestPoint + position, finalNormal + _lowestPoint + position);
        //Debug.DrawLine(_lowestPoint, _lowestPoint + transform.up);
        
        /* Save last rotation & position */
        _lastPos = position;
        _lastRot = rotation.eulerAngles;
    }

    /* Slowly reduces the movement to add over time */
    private void UpdateRecovery()
    {
        _time += Time.deltaTime;
        /* As time advances this tends to 0 */
        _moveAmountToAdd.x = Mathf.Lerp(_moveAmountToAdd.x, 0, Time.deltaTime * RecoverySpeed);
        _moveAmountToAdd.y = Mathf.Lerp(_moveAmountToAdd.y, 0, Time.deltaTime * RecoverySpeed);
    }

    /* Calculates the velocity of the liquid for the next frame */
    private void CalculateLiquidVelocity(Vector3 Position, Quaternion Rotation)
    {
        /* Calculate the liquid velocity and add it to the movement amount*/
        var velocity = (_lastPos - Position) / Time.deltaTime;
        var angularVelocity = Rotation.eulerAngles - _lastRot;
        _moveAmountToAdd.x += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxMovement, -MaxMovement, MaxMovement);
        _moveAmountToAdd.y += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxMovement, -MaxMovement, MaxMovement);
    }

    /* Sends the plane information to the shader */
    private void SendPlaneInformation(Vector3 FinalNormal)
    {
        /* Send plane information */
        _liquidMaterial.SetVector(PlaneNormal, FinalNormal.normalized);
        _liquidMaterial.SetVector(PlanePoint, _lowestPoint + FinalNormal + transform.position);
    }

    private Vector3 CalculateLiquidNormal()
    {
        /* Calculate the movement factors using sine waves */
        var waveMultiplier = 2 * Mathf.PI * WaveSpeed;
        var movementX = _moveAmountToAdd.x * Mathf.Sin(waveMultiplier * _time);
        var movementZ = _moveAmountToAdd.y * Mathf.Sin(waveMultiplier * _time);

        /* Calculate rotated directions depending on the movement */
        var directionX = Quaternion.Euler(MaxMovementAngle, 0, 0) * Vector3.up;
        var directionZ = new Vector3(directionX.y, directionX.z, directionX.x);
        
        /* Calculate the plane normal */
        var normalDirection = (Vector3.up + movementZ * directionX + movementX * directionZ).normalized;
        return normalDirection * (_meshHeight * VolumePercentage);
    }

    /* Finds the lowest point of the liquid, so as to build the plane from there */
    private void UpdateLowestPointIfNecessary(Quaternion Rotation)
    {
        var delta = Time.deltaTime * 8f;
        _lowestPoint.x = Mathf.Lerp(_lowestPoint.x, _newLowestPoint.x, delta);
        _lowestPoint.y = Mathf.Lerp(_lowestPoint.y, _newLowestPoint.y, delta);
        _lowestPoint.z = Mathf.Lerp(_lowestPoint.z, _newLowestPoint.z, delta);
        var euler = Rotation.eulerAngles;
        if (Math.Abs(euler.x - _lastRot.x) > 0.005f || Math.Abs(euler.y - _lastRot.y) > 0.005f || Math.Abs(euler.z - _lastRot.z) > 0.005f)
        {
            ForceUpdateLowestPoint();
        }
    }

    private void ForceUpdateLowestPoint()
    {
        var position = transform.position;
        var transformedVertices = _vertices.Select(T => transform.TransformPoint(T) - position).ToArray();
        _newLowestPoint = SupportPoint(transformedVertices, -Vector3.up);
    }
}