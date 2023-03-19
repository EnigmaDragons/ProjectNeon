using UnityEngine;

public class RecordHasSeenAlgeronFinalBossOnAwake : MonoBehaviour
{
    [SerializeField] private GameObject showTarget;
    
    private void Awake()
    {
        var shouldShow = !CurrentProgressionData.Data.HasSeenAlgeronFinalBoss;
        if (showTarget != null)
            showTarget.SetActive(shouldShow);
        if (shouldShow)
            CurrentProgressionData.Mutate(x => x.HasSeenAlgeronFinalBoss = true);
    }
}
