using UnityEngine;

public class DebugEnabledDisabled : MonoBehaviour
{
    #if UNITY_EDITOR
    private void OnEnable()
    {
        Log.Info($"{gameObject.name} - Enabled");
    }
    
    private void OnDisable()
    {
        Log.Info($"{gameObject.name} - Disabled");
    }
    #endif
}
