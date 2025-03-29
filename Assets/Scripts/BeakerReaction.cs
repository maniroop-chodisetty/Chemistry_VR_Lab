using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class BeakerReaction : MonoBehaviour
{
    public XRLever lever;
    public LiquidPhysics liquid;
    public float firstValue = 0.362f;
    public float secondValue = 0.721f;
    public float addLiquid = 0.0362f;
    public WaterDrop waterDrop;
    public GameObject reactionEffect;
    public float duration = 1f; // Duration of the animation

    public bool once;
    public GameObject clearLiquid;
    
    private void AddLiquidWithTween()
    {
        // Calculate the target volume after adding the liquid
        float targetVolume = liquid.VolumePercentage + addLiquid;

        // Create a tween to smoothly animate the addition of liquid volume
        DOTween.To(() => liquid.VolumePercentage, x => liquid.VolumePercentage = x, targetVolume, duration)
            .SetEase(Ease.Linear);
    }

    public void UpdateBeakerLiquid(bool firstLiquid, ParticleSystem water)
    {
        if (firstLiquid)
        {
            if (liquid.VolumePercentage < firstValue && firstLiquid)
            {
                AddLiquidWithTween();
                water.Play();
            }
            else
            {
                waterDrop.enabled = true;
            }
        }
        else if (liquid.VolumePercentage < secondValue && !firstLiquid)
        {
            AddLiquidWithTween();
            water.Play();
        }
        else
        {
            if (!once)
            {
                once = true;
                // Start the color change coroutine
                StartCoroutine(ChangeColorCoroutine());
            }
        }
    }
    
    private IEnumerator ChangeColorCoroutine()
    {
        yield return new WaitForSeconds(2f);
        reactionEffect.SetActive(true);
        clearLiquid.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        
    }
    
}
