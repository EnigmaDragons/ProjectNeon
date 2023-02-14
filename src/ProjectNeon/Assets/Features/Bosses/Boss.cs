using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Boss")]
public class Boss : ScriptableObject
{
    [SerializeField] private SpecificEncounterSegment stage1;
    [SerializeField] private SpecificEncounterSegment stage2;
    [SerializeField] private SpecificEncounterSegment stage3;

    public SpecificEncounterSegment Stage(int stage)
    {
        if (stage == 1)
            return stage1;
        else if (stage == 2)
            return stage2;
        else if (stage == 3)
            return stage3;
        Log.Warn($"An invalid stage was asked for: {stage}");
        return stage1;
    }
}