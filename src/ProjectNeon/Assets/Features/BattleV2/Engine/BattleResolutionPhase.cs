using UnityEngine;

public class BattleResolutionPhase : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private FloatReference delay = new FloatReference(1.5f);
    
    public void Execute()
    {
        resolutionZone.Resolve(this, delay);
    }
}
