using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketTagInteractor : XRSocketInteractor
{
    public string targetTag;
    public GameObject titleObj;
    public ParticleSystem water;
    public Experiment3 experiment;
    public Experiment4 experiment4;
    public Experiment5 experiment5;
    public LiquidPhysics liquid;
    public float duration = 1f; // Duration of the animation

    public float liquidAmount = 0.35f;

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        if (titleObj)
        {
            titleObj.SetActive(false);
        }

        if (water)
        {
            if (liquid)
            {
                AddLiquidWithTween();
            }
            water.Play();
            water = null;
            if (experiment)
            {
                experiment.count++;
            }
            if(experiment4)
            {
                experiment4.count++;
            } 
            if(experiment5)
            {
                experiment5.count++;
            }
        }
        return base.CanHover(interactable) && interactable.transform.tag == targetTag;
        
    }
    
    //public override bool CanSelect(IXRSelectInteractable interactable)
    //{
        // base.CanSelect(interactable) && interactable.transform.tag == targetTag;
    //}
    
    private void AddLiquidWithTween()
    {
        // Calculate the target volume after adding the liquid
        float targetVolume = liquid.VolumePercentage + liquidAmount;

        // Create a tween to smoothly animate the addition of liquid volume
        DOTween.To(() => liquid.VolumePercentage, x => liquid.VolumePercentage = x, targetVolume, duration)
            .SetEase(Ease.Linear);
    }

    protected override void OnDisable()
    {
        GameObject obj = attachTransform.gameObject;
        attachTransform = null;
        Destroy(obj);
    }
}
