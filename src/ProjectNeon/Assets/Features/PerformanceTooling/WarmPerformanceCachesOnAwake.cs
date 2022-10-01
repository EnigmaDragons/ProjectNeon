using UnityEngine;

public class WarmPerformanceCachesOnAwake : MonoBehaviour
{
    private void Awake()
    {
        var _ = LookupInitializer.Init;
    }
}
