using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgressV4")]
public class AdventureProgressV4 : AdventureProgressBase
{
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentGlobalEffects currentGlobalEffects;
    [SerializeField] private int currentChapterIndex;
    [SerializeField] private int currentSegmentIndex;
    [SerializeField] private int rngSeed = Rng.NewSeed();

    private DictionaryWithDefault<string, bool> _storyStates = new DictionaryWithDefault<string, bool>(false);

    public override int AdventureId => currentAdventure.Adventure.Id;
    public override string AdventureName => currentAdventure.Adventure.Title;
    public override CurrentGlobalEffects GlobalEffects => currentGlobalEffects;
    public override int CurrentStageProgress => currentSegmentIndex;
    public override int CurrentChapterNumber => currentChapterIndex + 1;
    private float Progress => CurrentStageProgress < 1 || CurrentChapter == null || CurrentChapter.SegmentCount < 1 ? 0f : (float)CurrentStageProgress / CurrentChapter.SegmentCount;
    public bool IsFinalStage => currentChapterIndex == currentAdventure.Adventure.StagesV4.Length - 1;
    public bool IsLastSegmentOfStage => currentSegmentIndex + 1 == CurrentStageLength;
    public override GameAdventureProgressType AdventureType => GameAdventureProgressType.V4;
    public override int RngSeed => rngSeed;
    public override bool UsesRewardXp => false;
    public override float BonusXpLevelFactor => 0.33333f;
    public override bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    public override bool IsFinalBoss => IsFinalStageSegment;
    public override float ProgressToBoss => CurrentStageProgress < 1 || CurrentChapter == null || CurrentChapter.SegmentCount < 1 ? 0f : (float)CurrentStageProgress / CurrentChapter.SegmentCountToBoss;
    public override float[] RisingActionPoints => new float[0];
    private int CurrentStageLength => CurrentChapter.SegmentCount;
    public override Difficulty Difficulty
    {
        get => Difficulty.Experienced;
        set { }
    }

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
    private float CombatProgress => CurrentChapter.CombatProgress(CurrentStageProgress);
    public override int CurrentPowerLevel => HasBegun ? Int(CurrentChapter.GetPowerLevel(CombatProgress) * GlobalPowerLevelFactor) : 120;
    public override int CurrentElitePowerLevel => Int(CurrentChapter.GetElitePowerLevel(CombatProgress) * GlobalPowerLevelFactor);
    private bool HasBegun => currentChapterIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && Progress >= CurrentStageLength;

    public override void Init(Adventure adventure, int chapterIndex) => Init(adventure, chapterIndex, 0);
    public override void Init(Adventure adventure, int chapterIndex, int segmentIndex)
    {        currentAdventure.Adventure = adventure;
        Reset();
        currentChapterIndex = chapterIndex;
        currentSegmentIndex = segmentIndex;
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
            AdventureId = AdventureId,
            Type = GameAdventureProgressType.V4,
            CurrentChapterIndex = currentChapterIndex,
            CurrentSegmentIndex = currentSegmentIndex,
            CurrentChapterFinishedHeatUpEvents = new int[0],
            FinishedStoryEvents = new string[0],
            PlayerReadMapPrompt = true,
            ActiveGlobalEffectIds = GlobalEffects.Value.Select(g => g.Data.OriginatingId).ToArray(),
            RngSeed = rngSeed,
            States = _storyStates.Keys.ToArray(),
            StateValues = _storyStates.Values.ToArray()
        };
    
    public bool InitAdventure(GameAdventureProgressData d, Adventure adventure)
    {
        Init(adventure, d.CurrentChapterIndex);
        currentSegmentIndex = d.CurrentSegmentIndex;
        ApplyGlobalEffects(d.ActiveGlobalEffectIds);
        rngSeed = d.RngSeed;
        _storyStates = new DictionaryWithDefault<string, bool>(false);
        for (var i = 0; i < d.States.Length; i++)
            _storyStates[d.States[i]] = d.StateValues[i];
        return true;
    }

    public override void Advance()
    {
        currentSegmentIndex++;
        rngSeed = Rng.NewSeed();;
        AdvanceStageIfNeeded();
    }

    public override void SetStoryState(string state, bool value) => _storyStates[state] = value;
    public override bool IsTrue(string state) => _storyStates[state];
}
