﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgress2")]
public class AdventureProgress2 : AdventureProgressBase
{
    [SerializeField] private int rngSeed = Rng.NewSeed();
    [SerializeField] private CurrentGameMap3 currentMap3;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentGlobalEffects currentGlobalEffects;
    [SerializeField] private int currentChapterIndex;
    [SerializeField] private List<string> finishedStoryEvents = new List<string>();
    [SerializeField] private List<int> finishedCurrentStageHeatUpEvents = new List<int>();
    [SerializeField] private bool playerReadMapPrompt = false;

    public override int AdventureId => currentAdventure.Adventure.Id;
    public Adventure CurrentAdventure => currentAdventure.Adventure;
    public override CurrentGlobalEffects GlobalEffects => currentGlobalEffects;
    public override int CurrentChapterNumber => currentChapterIndex + 1;
    public int CurrentChapterIndex => currentChapterIndex;
    public override int CurrentStageProgress => currentMap3.Progress;
    public float ProgressToUnlockChapterBoss => CurrentStageProgress == 0 ? 0f : (float)CurrentStageProgress / CurrentChapter.SegmentCount;
    public override float ProgressToBoss => ProgressToUnlockChapterBoss;
    public override float[] RisingActionPoints => new float[0];
    public bool IsFinalStage => currentChapterIndex == currentAdventure.Adventure.DynamicStages.Length - 1;
    public bool IsLastSegmentOfStage => currentMap3.CompletedNodes.Any() && currentMap3.CompletedNodes[currentMap3.CompletedNodes.Count - 1].Type == MapNodeType.Boss;
    public override string AdventureName => CurrentAdventure.Title;
    public override GameAdventureProgressType AdventureType => GameAdventureProgressType.V2;
    public override int RngSeed => currentMap3.CurrentNodeRngSeed;
    public override bool UsesRewardXp { get; } = true;
    public override float BonusXpLevelFactor { get; } = 0;
    public override bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    public override bool IsFinalBoss => IsFinalStageSegment;
    public string[] FinishedStoryEvents => finishedStoryEvents.ToArray();
    public bool PlayerReadMapPrompt => playerReadMapPrompt;
    public override Difficulty Difficulty
    {
        get => Difficulty.Experienced;
        set { }
    }

    public int[] FinishedCurrentStageHeatUpEvents => finishedCurrentStageHeatUpEvents.ToArray();

    public Indexed<HeatUpEventV0>[] RemainingHeatUpEvents => CurrentChapter.HeatUpEvents
        .Select((x, i) => new Indexed<HeatUpEventV0> {Index = i, Value = x})
        .Where(x => !finishedCurrentStageHeatUpEvents.Contains(x.Index))
        .ToArray();
    
    public Maybe<Indexed<HeatUpEventV0>> TriggeredHeatUpEvent => RemainingHeatUpEvents
        .Where(x => x.Value.ProgressThreshold <= ProgressToUnlockChapterBoss)
        .FirstOrMaybe();
    
    public DynamicStage CurrentChapter
    {
        get { 
            if (currentChapterIndex < 0 || currentChapterIndex >= currentAdventure.Adventure.DynamicStages.Length)
                Log.Error($"V2 Adventure Stage is illegal. {this}");
            return currentAdventure.Adventure.DynamicStages[currentChapterIndex]; 
        }
    }
    
    public override IStage Stage => CurrentChapter;

    public BossDetails BossDetails => new BossDetails
    {
        Battlefield = CurrentChapter.BossBattlefield,
        Enemies = CurrentChapter.BossEnemies,
        CurrentChapterNumber = CurrentChapterNumber
    };

    private static int Int(float f) => f.CeilingInt();
    private float GlobalPowerLevelFactor => currentGlobalEffects.EncounterDifficultyFactor;
    public override int CurrentPowerLevel => Int(CurrentChapter.GetPowerLevel(((float)currentMap3.Progress + 1) / CurrentChapter.SegmentCount) * GlobalPowerLevelFactor);
    public override int CurrentElitePowerLevel => Int(CurrentChapter.GetElitePowerLevel(((float)currentMap3.Progress + 1) / CurrentChapter.SegmentCount) * GlobalPowerLevelFactor);
    private bool HasBegun => currentChapterIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && currentMap3.CompletedNodes.Any() && currentMap3.CompletedNodes.Last().Type == MapNodeType.Boss;

    public override void Init(Adventure adventure, int chapterIndex)
    {
        currentAdventure.Adventure = adventure;
        Reset();
        currentChapterIndex = chapterIndex;
        Log.Info($"Init Adventure. {this}");
    }

    public override void Init(Adventure adventure, int chapterIndex, int segmentIndex) => Init(adventure, chapterIndex);

    public void ApplyGlobalEffects(int[] effectIds)
    {
        var ctx = new GlobalEffectContext(currentGlobalEffects);
        effectIds.ForEach(id => currentGlobalEffects.ApplyById(id, ctx));
    }
    
    public override string ToString() =>
        $"Adventure: {currentAdventure.name}. Stage: {currentChapterIndex}. StageProgress: {currentMap3.Progress}";

    public override void InitIfNeeded()
    {
        if (HasBegun) return;
        
        Log.Info($"Is advancing the adventure. {this}");
        AdvanceStageIfNeeded();
    }

    public void MarkMapPromptComplete()
    {
        playerReadMapPrompt = true;
        Message.Publish(new AutoSaveRequested());
    }
    
    public override void Reset()
    {
        currentChapterIndex = -1;
        finishedStoryEvents.Clear();
        currentGlobalEffects.Clear();
        finishedCurrentStageHeatUpEvents.Clear();
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
            finishedCurrentStageHeatUpEvents.Clear();
            currentMap3.SetMap(CurrentChapter.Map);
        } 
        else
        {
            Log.Info("Can't advance: is final stage");
        } 
    }

    public void RecordEncounteredStoryEvent(StoryEvent2 e) => finishedStoryEvents.Add(e.name);
    public void SetFinishedStoryEvents(string[] storyEvents) => finishedStoryEvents = storyEvents.ToList();

    public void RecordFinishedHeatUpEvent(int index) => finishedCurrentStageHeatUpEvents.Add(index);
    public void SetFinishedHeatUpEvents(int[] indexes) => finishedCurrentStageHeatUpEvents = indexes.ToList();
    
    public override LootPicker CreateLootPicker(PartyAdventureState party) 
        => new LootPicker(CurrentChapterNumber, CurrentChapterNumber > 0 ? CurrentChapter.RewardRarityFactors : new DefaultRarityFactors(), party);
    
    public override GameAdventureProgressData GetData()
        => new GameAdventureProgressData
        {
            AdventureId = AdventureId,
            Type = GameAdventureProgressType.V2,
            CurrentChapterIndex = CurrentChapterIndex,
            CurrentChapterFinishedHeatUpEvents = FinishedCurrentStageHeatUpEvents,
            FinishedStoryEvents = FinishedStoryEvents,
            PlayerReadMapPrompt = PlayerReadMapPrompt,
            ActiveGlobalEffectIds = GlobalEffects.Value.Select(g => g.Data.OriginatingId).ToArray(),
            RngSeed = rngSeed
        };
    
    public bool InitAdventure(GameAdventureProgressData d, Adventure adventure)
    {
        Init(adventure, d.CurrentChapterIndex);
        SetFinishedStoryEvents(d.FinishedStoryEvents);
        SetFinishedHeatUpEvents(d.CurrentChapterFinishedHeatUpEvents);
        ApplyGlobalEffects(d.ActiveGlobalEffectIds);
        rngSeed = d.RngSeed;
        if (d.PlayerReadMapPrompt)
            MarkMapPromptComplete();
        return true;
    }

    public override void Advance()
    {
        currentMap3.CompleteCurrentNode();
        rngSeed = Rng.NewSeed();
        AdvanceStageIfNeeded();
    } 
    
    public override void SetStoryState(string state, bool value) {}
    public override bool IsTrue(string state) => false;
}