using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class DisableGrabbingHandModel : MonoBehaviour
{
    public bool experiment1;
    
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public float duration = 1f;
    public string Text;
    public TextMeshPro instruction;
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Target scale for the animation
    public Ease easeType = Ease.OutQuad; // Easing type for the animation
    private Tween scaleTween; // Reference to the scale tween
    
    public bool experiment2;

    public GameObject flaskObjPos;


    public bool experiment5;
    public bool canPick;
    public GameObject info; 
    public GameObject beaker1; 
    public GameObject beaker2; 
    
    
    
    public GameObject leftHandModel;
    public GameObject rightHandModel;
    
    // Start is called before the first frame update
    private void Start()
    {
        if(experiment1)
        {
            instruction.text = Text;
            StartScaleAnimation();
            
            // Store the original position and rotation when the object is instantiated
            originalPosition = transform.position;
            originalRotation = transform.rotation;

        }
        
        var grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(HideGrabbingHand);
        grabInteractable.selectExited.AddListener(ShowGrabbingHand);
    }

    private void OnEnable()
    {
        leftHandModel = GameObject.FindGameObjectWithTag("LeftHand");
        rightHandModel = GameObject.FindGameObjectWithTag("RightHand");
    }

    private void HideGrabbingHand(SelectEnterEventArgs arg)
    {
        switch (arg.interactorObject.transform.tag)
        {
            case "Left Hand":
                leftHandModel.SetActive(false);
                break;
            case "Right Hand":
                rightHandModel.SetActive(false);
                break;
        }

        if (experiment5 && info)
        {
            info.SetActive(true);
            Invoke(nameof(HideInfo), 5f);
        }
        
        if (experiment5 && beaker1)
        {
            beaker1.SetActive(false);
        }
        if (experiment5 && beaker2)
        {
            beaker2.SetActive(false);
        }

        if (experiment1)
        {
            StopScaleAnimation();
        }
    }

    private void ShowGrabbingHand(SelectExitEventArgs arg)
    {
        switch (arg.interactorObject.transform.tag)
        {
            case "Left Hand":
                leftHandModel.SetActive(true);
                break;
            case "Right Hand":
                rightHandModel.SetActive(true);
                break;
        }

        if (experiment1)
        {
            ResetObject();
        }
        else if (experiment2)
        {
            MoveToPos();
        }
    }

    // Experiment 1
    
    private void ResetObject()
    {
        // Move the object back to its original position and rotation using DOTween
        transform.DOMove(originalPosition, duration);
        transform.DORotateQuaternion(originalRotation, duration).OnComplete(StartScaleAnimation);
    }
    
    private void StartScaleAnimation()
    { 
        if (instruction)
        {
            instruction.gameObject.SetActive(true);
        }
        // Scale up animation
        scaleTween = instruction.transform.DOScale(targetScale, duration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                // Scale down animation
                scaleTween = instruction.transform.DOScale(Vector3.one, duration)
                    .SetEase(easeType)
                    .OnComplete(StartScaleAnimation);
            });
    }

    private void StopScaleAnimation()
    {
        if (instruction)
        {
            instruction.gameObject.SetActive(false);
        }

        scaleTween?.Kill();
    }
    
    // Experiment 2

    private void MoveToPos()
    {
        transform.DOMove(flaskObjPos.transform.position, duration);
        transform.DORotateQuaternion(flaskObjPos.transform.localRotation, duration).OnComplete(StartScaleAnimation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Flask Pos")
        {
            flaskObjPos = other.gameObject;
            flaskObjPos.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Flask Pos")
        {
            flaskObjPos.transform.GetChild(0).gameObject.SetActive(false);
            flaskObjPos = null;
        }
    }

    public void HideInfo()
    {
        info.SetActive(false);
    }
}
