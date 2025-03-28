using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Experiment5 : MonoBehaviour
{
    public GameObject popupUI;
    public int count = 0;
    public TextMeshPro info;
    public GameObject beaker1;
    public GameObject beaker2;
    public GameObject beaker3;
    
    public DisableGrabbingHandModel disableGrabbingHandModel;
    public GameObject enemy; 
    
    public TextMeshPro timerText; // Reference to the TextMeshPro text component
    private float totalTime = 120f; // Total time in seconds (2 minutes)
    private float elapsedTime = 0f; // Elapsed time since the timer started

    private void Start()
    {
        Invoke(nameof(HidePopupUI), 4f);
    }

    private void Update()
    {
        if (count == 1)
        {
            info.text = "Now Look for next element to make explosive ";
        }

        if (count == 2)
        {
            if (enemy)
            {
                enemy.SetActive(true);
            }
            disableGrabbingHandModel.enabled = true;
            count++;
        }
    }

    public void HidePopupUI()
    {
        beaker1.SetActive(true);
        beaker2.SetActive(true);
        beaker3.SetActive(true);
        popupUI.SetActive(false);
        info.text = "Look for the clue to find the elements to make explosive ";
        StartCoroutine(TimerCoroutine());
    }
    
    private IEnumerator TimerCoroutine()
    {
        while (elapsedTime < totalTime)
        {
            // Update the timer text with the remaining time
            UpdateTimerText(totalTime - elapsedTime);

            // Wait for the next frame
            yield return null;

            // Increment elapsed time by the time taken for the last frame
            elapsedTime += Time.deltaTime;
        }

        // Ensure that the timer text shows 00:00 after the timer finishes
        UpdateTimerText(0f);
        if (enemy)
        {
            enemy.SetActive(true);
        }

        // After 2 minutes, do something
        Debug.Log("Two minutes have passed!");
    }

    private void UpdateTimerText(float remainingTime)
    {
        // Format the remaining time as minutes and seconds
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        // Update the TextMeshPro text component with the formatted time
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnDisable()
    {
        beaker1.SetActive(false);
        beaker2.SetActive(false);
    }

    
}
