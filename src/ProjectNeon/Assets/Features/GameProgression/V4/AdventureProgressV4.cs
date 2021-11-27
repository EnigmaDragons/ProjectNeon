using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgressV4")]
public class AdventureProgressV4 : AdventureProgressBase
{
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentGlobalEffects currentGlobalEffects;
    [SerializeField] private int currentChapterIndex;
    [SerializeField] private int currentSegmentIndex;
    
    public Adventure CurrentAdventure => currentAdventure.Adventure;
    public override CurrentGlobalEffects GlobalEffects => currentGlobalEffects;
    public int CurrentAdventureId => currentAdventure.Adventure.Id;
    public int CurrentStageProgress => currentSegmentIndex;
    public override int CurrentChapterNumber => currentChapterIndex + 1;
    public int CurrentChapterIndex => currentChapterIndex;
    private float Progress => CurrentStageProgress == 0 ? 0f : (float)CurrentStageProgress / CurrentChapter.SegmentCount;
    public float ProgressToUnlockChapterBoss => Progress;
    public bool IsFinalStage => currentChapterIndex == currentAdventure.Adventure.StagesV4.Length - 1;
    public bool IsLastSegmentOfStage => currentSegmentIndex + 1 == CurrentStageLength;
    public override bool UsesRewardXp { get; } = false;
    public override float BonusXpLevelFactor { get; } = 0.33333f;
    public override bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    private int CurrentStageLength => CurrentChapter.SegmentCount;
    
    public StaticStageV4 CurrentChapter
    {
        get { 
            if (currentChapterIndex < 0 || currentChapterIndex >= currentAdventure.Adventure.StagesV4.Length)
                Log.Error($"Adventure Stage is illegal. {this}");
            return currentAdventure.Adventure.StagesV4[currentChapterIndex]; 
        }
    }

    public StageSegment CurrentStageSegment => CurrentChapter.Segments[currentSegmentIndex];

    public override IStage Stage => CurrentChapter;

    private static int Int(float f) => f.CeilingInt();
    private float GlobalPowerLevelFactor => currentGlobalEffects.EncounterDifficultyFactor;
    public override int CurrentPowerLevel => Int(CurrentChapter.GetPowerLevel(((float)Progress + 1) / CurrentChapter.SegmentCount) * GlobalPowerLevelFactor);
    public override int CurrentElitePowerLevel => Int(CurrentChapter.GetElitePowerLevel(((float)Progress + 1) / CurrentChapter.SegmentCount) * GlobalPowerLevelFactor);
    private bool HasBegun => currentChapterIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && Progress >= CurrentStageLength;

    public override void Init(Adventure adventure, int chapterIndex)
    {
        currentAdventure.Adventure = adventure;
        Reset();
        currentChapterIndex = chapterIndex;
        currentSegmentIndex = 0;
        Log.Info($"Init Adventure. {this}");
    }

    public void ApplyGlobalEffects(int[] effectIds)
    {
        var ctx = new GlobalEffectContext(currentGlobalEffects);
        effectIds.ForEach(id => currentGlobalEffects.ApplyById(id, ctx));
    }
    
    public override string ToString() =>
        $"Adventure: {currentAdventure.name}. Stage: {currentChapterIndex}. StageProgress: {Progress}";

    public override void InitIfNeeded()
    {
        if (HasBegun) return;
        
        Log.Info($"Is advancing the adventure. {this}");
        AdvanceStageIfNeeded();
    }
    
    public override void Reset()
    {
        currentChapterIndex = -1;
        currentGlobalEffects.Clear();
        Message.Publish(new AdventureProgressChanged());
    }

    public override void AdvanceStageIfNeeded()
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

    public override LootPicker CreateLootPicker(PartyAdventureState party) 
        => new LootPicker(CurrentChapterNumber, CurrentChapterNumber > 0 ? CurrentChapter.RewardRarityFactors : new DefaultRarityFactors(), party);

    public override GameAdventureProgressData GetData()
        => new GameAdventureProgressData
        {
            AdventureId = CurrentAdventureId,
            Type = GameAdventureProgressType.V4,
            CurrentChapterIndex = currentChapterIndex,
            CurrentSegmentIndex = currentSegmentIndex,
            CurrentChapterFinishedHeatUpEvents = new int[0],
            FinishedStoryEvents = new string[0],
            PlayerReadMapPrompt = true,
            ActiveGlobalEffectIds = GlobalEffects.Value.Select(g => g.Data.OriginatingId).ToArray()
        };
    
    public bool InitAdventure(GameAdventureProgressData adventureProgress, Adventure adventure)
    {
        Init(adventure, adventureProgress.CurrentChapterIndex);
        currentSegmentIndex = adventureProgress.CurrentSegmentIndex;
        ApplyGlobalEffects(adventureProgress.ActiveGlobalEffectIds);
        return true;
    }

    public override void Advance()
    {
        currentSegmentIndex++;
        AdvanceStageIfNeeded();
    }
}
