using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgress2")]
public class AdventureProgress2 : ScriptableObject
{
    [SerializeField] private CurrentGameMap3 currentMap3;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentGlobalEffects currentGlobalEffects;
    [SerializeField] private int currentChapterIndex;
    [SerializeField] private List<string> finishedStoryEvents = new List<string>();
    [SerializeField] private List<int> finishedCurrentStageHeatUpEvents = new List<int>();
    [SerializeField] private bool playerReadMapPrompt = false;
    
    public Adventure CurrentAdventure => currentAdventure.Adventure;
    public CurrentGlobalEffects GlobalEffects => currentGlobalEffects;
    public int CurrentAdventureId => currentAdventure.Adventure.Id;
    public int CurrentChapterNumber => currentChapterIndex + 1;
    public int CurrentChapterIndex => currentChapterIndex;
    public int CurrentStageProgress => currentMap3.Progress;
    public float ProgressToUnlockChapterBoss => CurrentStageProgress == 0 ? 0f : (float)CurrentStageProgress / CurrentChapter.SegmentCount;
    public bool IsFinalStage => currentChapterIndex == currentAdventure.Adventure.DynamicStages.Length - 1;
    public bool IsLastSegmentOfStage => currentMap3.CompletedNodes.Any() && currentMap3.CompletedNodes[currentMap3.CompletedNodes.Count - 1].Type == MapNodeType.Boss;
    public bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    public string[] FinishedStoryEvents => finishedStoryEvents.ToArray();
    public bool PlayerReadMapPrompt => playerReadMapPrompt;

    public int[] FinishedCurrentStageHeatUpEvents => finishedCurrentStageHeatUpEvents.ToArray();
    public Maybe<Indexed<HeatUpEventV0>> TriggeredHeatUpEvent => CurrentChapter.HeatUpEvents
        .Select((x, i) => new Indexed<HeatUpEventV0> { Index = i, Value = x})
        .Where(x => !finishedCurrentStageHeatUpEvents.Contains(x.Index) && x.Value.ProgressThreshold <= ProgressToUnlockChapterBoss)
        .FirstOrMaybe();
    
    public DynamicStage CurrentChapter
    {
        get { 
            if (currentChapterIndex < 0 || currentChapterIndex >= currentAdventure.Adventure.DynamicStages.Length)
                Log.Error($"Adventure Stage is illegal. {this}");
            return currentAdventure.Adventure.DynamicStages[currentChapterIndex]; 
        }
    }

    private static int Int(float f) => f.CeilingInt();
    private float GlobalPowerLevelFactor => currentGlobalEffects.EncounterDifficultyFactor;
    public int CurrentPowerLevel => Int(CurrentChapter.GetPowerLevel(((float)currentMap3.Progress + 1) / CurrentChapter.SegmentCount) * GlobalPowerLevelFactor);
    public int CurrentElitePowerLevel => Int(CurrentChapter.GetElitePowerLevel(((float)currentMap3.Progress + 1) / CurrentChapter.SegmentCount) * GlobalPowerLevelFactor);
    private bool HasBegun => currentChapterIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && currentMap3.CompletedNodes.Any() && currentMap3.CompletedNodes.Last().Type == MapNodeType.Boss;

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

    public void ApplyGlobalEffects(GlobalEffectData[] effects)
    {
        var ctx = new GlobalEffectContext(currentGlobalEffects);
        effects.ForEach(e => currentGlobalEffects.Apply(AllGlobalEffects.Create(e), ctx));
    }
    
    public override string ToString() =>
        $"Adventure: {currentAdventure.name}. Stage: {currentChapterIndex}. StageProgress: {currentMap3.Progress}";

    public void InitIfNeeded()
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
    
    public void Reset()
    {
        currentChapterIndex = -1;
        finishedStoryEvents.Clear();
        currentGlobalEffects.Clear();
        finishedCurrentStageHeatUpEvents.Clear();
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
            finishedCurrentStageHeatUpEvents.Clear();
            currentMap3.SetMap(CurrentChapter.Map);
        } else
        {
            Log.Info("Can't advance: is final stage");
        } 
    }

    public void RecordEncounteredStoryEvent(StoryEvent2 e) => finishedStoryEvents.Add(e.name);
    public void SetFinishedStoryEvents(string[] storyEvents) => finishedStoryEvents = storyEvents.ToList();

    public void RecordFinishedHeatUpEvent(int index) => finishedCurrentStageHeatUpEvents.Add(index);
    public void SetFinishedHeatUpEvents(int[] indexes) => finishedCurrentStageHeatUpEvents = indexes.ToList();
    
    public LootPicker CreateLootPicker(PartyAdventureState party) 
        => new LootPicker(CurrentChapterNumber, CurrentChapterNumber > 0 ? CurrentChapter.RewardRarityFactors : new DefaultRarityFactors(), party);
}