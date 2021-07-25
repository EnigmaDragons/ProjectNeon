using UnityEngine;

public class InitMetrics : MonoBehaviour
{
    [SerializeField] private StringReference appName;
    [SerializeField] private StringReference version;
    
    private void Awake()
    {
        ErrorReport.Init(appName, version);
        AllMetrics.Init(version, InstallId.Get());
    }
}
