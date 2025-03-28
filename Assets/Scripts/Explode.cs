using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public ParticleSystem fire;
    public GameObject hideObj;
    public MeshRenderer beakerMesh;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            fire.gameObject.SetActive(true);
            other.gameObject.SetActive(false);
            beakerMesh.enabled = false;
            hideObj.SetActive(false);

        }
    }
}
