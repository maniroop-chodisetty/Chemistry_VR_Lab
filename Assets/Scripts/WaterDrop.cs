using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class WaterDrop : MonoBehaviour
{
    public XRLever lever;
    public ParticleSystem water;
    public bool firstLiquid;
    public bool secondLiquid;
    public BeakerReaction beakerReaction;
    public bool leverValue;
    public DisableGrabbingHandModel disableGrabbingHandModel;
    public GameObject triggerObj;

    private void Update()
    {
        if (!lever.value) return;
        if (!leverValue) return;
        if (triggerObj != disableGrabbingHandModel.flaskObjPos) return;
        leverValue = false;
        lever.value = false;
        if (firstLiquid)
        {
            beakerReaction.UpdateBeakerLiquid(true, water);
        }
        else if (secondLiquid)
        {
            beakerReaction.UpdateBeakerLiquid(false, water);
        }

        Invoke(nameof(ResetLever), 2f);
    }

    public void ResetLever()
    {
        leverValue = true;
    }
}
