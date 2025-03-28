using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class Experiment4 : MonoBehaviour
{
    public XRLever lever;
    public int count = 0;
    public bool canPull;
    public GameObject leverText;
    public GameObject beaker2ndText;
    public GameObject beaker1;
    public GameObject beaker2;
    public ParticleSystem burnerFire;
    public ParticleSystem fire;

    private void Update()
    {
        if (count == 1)
        {
            leverText.SetActive(true);
            count++;
            canPull = true;
        }

        if (count == 4)
        {
            Invoke(nameof(Reset2), 1f);
            count++;
        }

        if (canPull)
        {
            if (!lever.value) return;
            {
                canPull = false;
                lever.value = false;
                beaker1.SetActive(false);
                burnerFire.Play();
                Invoke(nameof(Reset), 3f);
            }
        }
    }

    public void Reset()
    {
        burnerFire.Stop();
        leverText.SetActive(false);
        count++;
        beaker2ndText.SetActive(true);
    }   
    
    public void Reset2()
    {
        fire.gameObject.SetActive(true);
        beaker2.SetActive(false);
        fire.Play();
    }

    private void OnDisable()
    {
        beaker1.SetActive(false);
        beaker2.SetActive(false);
    }

    private void OnEnable()
    {
        beaker1.SetActive(true);
        beaker2.SetActive(true);
    }
}
