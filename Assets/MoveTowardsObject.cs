using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveTowardsObject : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public GameObject lose;
    public GameObject win;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Check if the target object is assigned
        if (target != null)
        {
            // Calculate the direction from the current position to the target position
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;

            // Move towards the target object at a constant speed
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            Debug.LogWarning("Target object is not assigned.");
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lose.SetActive(true);
            Invoke(nameof(RestartScene), 5f);
            target.gameObject.GetComponent<XROrigin>().enabled = false;
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDisable()
    {
        win.SetActive(true);
    }
}
