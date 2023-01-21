using UnityEngine;

public class RecordHasSeenAlgeronFinalBossOnAwake : MonoBehaviour
{
    private void Awake() => CurrentProgressionData.Mutate(x => x.HasSeenAlgeronFinalBoss = true);
}
