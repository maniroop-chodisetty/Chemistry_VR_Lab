using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class Experiment3 : MonoBehaviour
{
    public XRLever lever;
    public Animator animator;
    public bool again;
    public int count = 0;
    public bool canPull;
    public GameObject leverText;
    public GameObject beaker1;
    public GameObject beaker2;

    private void Update()
    {
        if (count > 1 && !canPull)
        {
            leverText.SetActive(true);
            count++;
            canPull = true;
        }

        if (canPull)
        {
            if (!lever.value) return;
            beaker1.SetActive(false);
            beaker2.SetActive(false);
            lever.value = false;
            PlayAnimation();
        }
    }

    public void PlayAnimation()
    {
        if (again)
        {
            again = false;        
            animator.Play("experiment3");
        }
    }

    public void Reset()
    {
        again = true;
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
