using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgressV4")]
public class AdventureProgressV4 : ScriptableObject
{
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentGlobalEffects currentGlobalEffects;
    [SerializeField] private int currentChapterIndex;
    [SerializeField] private int currentSegmentIndex;

    public Adventure CurrentAdventure => currentAdventure.Adventure;
    public CurrentGlobalEffects GlobalEffects => currentGlobalEffects;
    public int CurrentAdventureId => currentAdventure.Adventure.Id;
    public int CurrentStageProgress => currentSegmentIndex;
    private int CurrentChapterNumber => currentChapterIndex + 1;
    public int CurrentChapterIndex => currentChapterIndex;
    private float Progress => CurrentStageProgress == 0 ? 0f : (float)CurrentStageProgress / CurrentChapter.SegmentCount;
    public float ProgressToUnlockChapterBoss => Progress;
    public bool IsFinalStage => currentChapterIndex == currentAdventure.Adventure.StagesV4.Length - 1;
    public bool IsLastSegmentOfStage => currentSegmentIndex + 1 == CurrentStageLength;
    public bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    private int CurrentStageLength => CurrentChapter.SegmentCount;
    
    public StaticStageV4 CurrentChapter
    {
        get { 
            if (currentChapterIndex < 0 || currentChapterIndex >= currentAdventure.Adventure.StagesV4.Length)
                Log.Error($"Adventure Stage is illegal. {this}");
            return currentAdventure.Adventure.StagesV4[currentChapterIndex]; 
        }
    }

    private static int Int(float f) => f.CeilingInt();
    private float GlobalPowerLevelFactor => currentGlobalEffects.EncounterDifficultyFactor;
    public int CurrentPowerLevel => Int(CurrentChapter.GetPowerLevel(((float)Progress + 1) / CurrentChapter.SegmentCount) * GlobalPowerLevelFactor);
    public int CurrentElitePowerLevel => Int(CurrentChapter.GetElitePowerLevel(((float)Progress + 1) / CurrentChapter.SegmentCount) * GlobalPowerLevelFactor);
    private bool HasBegun => currentChapterIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && Progress >= CurrentStageLength;

    public void Init()
    {
        Reset();
        Log.Info($"Init Adventure. {this}");
    }

    public void Init(Adventure adventure, int chapterIndex)
    {
        currentAdventure.Adventure = adventure;
        Init();
        currentChapterIndex = chapterIndex;
        Log.Info($"Init Adventure. {this}");
    }

    public void ApplyGlobalEffects(int[] effectIds)
    {
        var ctx = new GlobalEffectContext(currentGlobalEffects);
        effectIds.ForEach(id => currentGlobalEffects.ApplyById(id, ctx));
    }
    
    public override string ToString() =>
        $"Adventure: {currentAdventure.name}. Stage: {currentChapterIndex}. StageProgress: {Progress}";

    public void InitIfNeeded()
    {
        if (HasBegun) return;
        
        Log.Info($"Is advancing the adventure. {this}");
        AdvanceStageIfNeeded();
    }
    
    public void Reset()
    {
        currentChapterIndex = -1;
        currentGlobalEffects.Clear();
        Message.Publish(new AdventureProgressChanged());
    }

    public void AdvanceStageIfNeeded()
    {
        if (HasBegun && !CurrentStageIsFinished) 
            return;
        
        AdvanceStage();
        Log.Info(ToString());
        Message.Publish(new AdventureProgressChanged());
    }

    private void AdvanceStage()
    {
        if (!IsFinalStage)
        {
            currentChapterIndex++;
        } 
        else
        {
            Log.Info("Can't advance: is final stage");
        } 
    }

    public LootPicker CreateLootPicker(PartyAdventureState party) 
        => new LootPicker(CurrentChapterNumber, CurrentChapterNumber > 0 ? CurrentChapter.RewardRarityFactors : new DefaultRarityFactors(), party);
}
