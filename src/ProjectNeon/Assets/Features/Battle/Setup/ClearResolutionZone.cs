using UnityEngine;

public class ClearResolutionZone : MonoBehaviour
{
    [SerializeField] private CardPlayZone resolutionZone;
    
    void Awake()
    {
        resolutionZone.Clear();
    }
}
