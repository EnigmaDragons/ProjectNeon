using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgressV5")]
public class AdventureProgressV5 : AdventureProgressBase
{
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentGlobalEffects currentGlobalEffects;
    [SerializeField] private CurrentMapSegmentV5 currentMap;
    [SerializeField] private Library library;
    [SerializeField] private int currentChapterIndex;
    [SerializeField] private int currentSegmentIndex;
    [SerializeField] private int rngSeed = Rng.NewSeed();
    [SerializeField] private Difficulty difficulty;

    private DictionaryWithDefault<string, bool> _storyStates = new DictionaryWithDefault<string, bool>(false);

    public override int AdventureId => currentAdventure.Adventure.Id;
    public override string AdventureNameTerm => currentAdventure.Adventure.MapTitleTerm;
    public override CurrentGlobalEffects GlobalEffects => currentGlobalEffects;
    public override int CurrentStageProgress => currentSegmentIndex;
    public override int CurrentChapterNumber => currentChapterIndex + 1;
    private float Progress => CurrentStageProgress < 1 ? 0f : (float)CurrentStageProgress / CurrentChapter.SegmentCount;
    private bool IsFinalStage => currentChapterIndex >= currentAdventure.Adventure.StagesV5.Length - 1;
    private bool IsLastSegmentOfStage => currentSegmentIndex + 1 == CurrentStageLength;
    public override GameAdventureProgressType AdventureType => GameAdventureProgressType.V5;
    public override int RngSeed => rngSeed;
    public override bool UsesRewardXp => false;
    public override float BonusXpLevelFactor => currentAdventure.Adventure.BonusXpFactor;
    public override bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    public override bool IsFinalBoss => IsFinalStage && CurrentStageSegment.MapNodeType == MapNodeType.Boss;
    public override float ProgressToBoss => CurrentStageProgress < 1 ? 0f : (float)CurrentStageProgress / CurrentChapter.SegmentCountToBoss;
    private int CurrentStageLength => CurrentChapter.SegmentCount;    
    public override float[] RisingActionPoints => CurrentChapter.Segments
        .Select((x, i) => (x, i))
        .Where(y => y.x.MapNodeType == MapNodeType.Elite).Select(y => (float) y.i / CurrentChapter.SegmentCountToBoss)
        .ToArray();
    public override Difficulty Difficulty
    {
        get => difficulty;
        set => difficulty = value;
    }
    
    public HybridStageV5 CurrentChapter
    {
        get { 
            if (currentChapterIndex < 0 || currentChapterIndex >= currentAdventure.Adventure.StagesV5.Length)
                Log.Error($"Adventure Stage is illegal. Chapter Index {currentChapterIndex}");
            return currentAdventure.Adventure.StagesV5[currentChapterIndex]; 
        }
    }

    public StageSegment CurrentStageSegment => CurrentChapter.Segments[currentSegmentIndex];

    public StageSegment[] SecondarySegments
    {
        get
        {
            if (CurrentChapter == null)
                Log.Error("Current Chapter is null");
            else if (CurrentChapter.MaybeSecondarySegments == null)
                Log.Error("MaybeSecondarySegments is null");
            else if (CurrentChapter.MaybeStorySegments == null)
                Log.Error("MaybeStorySegments is null");
            var segments = new List<StageSegment>();
            if (CurrentChapter.MaybeSecondarySegments.Length > currentSegmentIndex && CurrentChapter.MaybeSecondarySegments[currentSegmentIndex] != null)
                segments.Add(CurrentChapter.MaybeSecondarySegments[currentSegmentIndex]);
            if (CurrentChapter.MaybeStorySegments.Length > currentSegmentIndex && CurrentChapter.MaybeStorySegments[currentSegmentIndex] != null)
                segments.Add(CurrentChapter.MaybeStorySegments[currentSegmentIndex]);
            return segments.ToArray();
        }
    }

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
    {        
        currentAdventure.Adventure = adventure;
        Reset();
        currentChapterIndex = chapterIndex;
        currentSegmentIndex = segmentIndex;
        currentMap.SetMap(CurrentChapter.Map);
        rngSeed = ConsumableRngSeed.GenerateNew().Peek.Value;
        difficulty = library.DefaultDifficulty;
        Log.Info($"Init Adventure. {this}");
    }
    
    public void ApplyGlobalEffects(int[] effectIds)
    {
        var ctx = new GlobalEffectContext(currentGlobalEffects);
        effectIds.ForEach(id => currentGlobalEffects.ApplyById(id, ctx));
    }

    public override string ToString()
    {
        try
        {
            return
                $"Adventure: {currentAdventure.Adventure.Id} {currentAdventure.Adventure.TitleTerm.ToEnglish()}. Stage: {currentChapterIndex}. StageProgress: {Progress}." +
                $"Chapter: {currentChapterIndex}/{currentAdventure.Adventure.StagesV5.Length}. Segment: {currentSegmentIndex}. Rng: {RngSeed}";
        }
        catch (Exception e)
        {
            return "Unable to Capture Adventure Details";
        }
    }

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
            currentMap.SetMap(CurrentChapter.Map);
            currentChapterIndex++;
        } 
        else
        {
            Log.Info("Can't advance: is final stage");
        } 
    }

    public override LootPicker CreateLootPicker(PartyAdventureState party) 
        => new LootPicker(CurrentChapterNumber, 
            CurrentChapterNumber > 0 ? CurrentChapter.RewardRarityFactors : new DefaultRarityFactors(), 
            party,
            new DeterministicRng(ConsumableRngSeed.Consume()));

    public override GameAdventureProgressData GetData() 
        => new GameAdventureProgressData
        {
             AdventureId = AdventureId,
             Type = GameAdventureProgressType.V5,
             CurrentChapterIndex = currentChapterIndex,
             CurrentSegmentIndex = currentSegmentIndex,
             CurrentChapterFinishedHeatUpEvents = new int[0],
             FinishedStoryEvents = new string[0],
             PlayerReadMapPrompt = true,
             ActiveGlobalEffectIds = GlobalEffects.Value.Select(g => g.Data.OriginatingId).ToArray(),
             RngSeed = rngSeed,
             States = _storyStates.Keys.ToArray(),
             StateValues = _storyStates.Values.ToArray(),
             DifficultyId = difficulty == null ? difficulty.Id : 0
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
        difficulty = library.DefaultDifficulty;
        return true;
    }

    public override void Advance()
    {
        currentSegmentIndex++;
        rngSeed = Rng.NewSeed();;
        AdvanceStageIfNeeded();
        AllMetrics.PublishAdventureProgress(AdventureNameTerm.ToEnglish(), Progress);
    }

    public override void SetStoryState(string state, bool value) => _storyStates[state] = value;
    public override bool IsTrue(string state) => _storyStates[state];
}
