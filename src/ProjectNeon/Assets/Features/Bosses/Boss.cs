using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Boss")]
public class Boss : ScriptableObject, ILocalizeTerms
{
    [SerializeField] public int id;
    [SerializeField] private SpecificEncounterSegment stage1;
    [SerializeField] private SpecificEncounterSegment stage2;
    [SerializeField] private SpecificEncounterSegment stage3;
    [SerializeField] private Sprite bust;
    
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

    public Sprite Bust => bust;
    public string NameTerm => $"Bosses/Boss{id.ToString().PadLeft(3, '0')}-Name";
    public string DescriptionTerm => $"Bosses/Boss{id.ToString().PadLeft(3, '0')}-Desc";
    public string[] GetLocalizeTerms() => new[] { NameTerm, DescriptionTerm };
}