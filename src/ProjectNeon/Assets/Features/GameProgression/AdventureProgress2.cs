using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgress2")]
public class AdventureProgress2 : ScriptableObject
{
    [SerializeField] private CurrentGameMap2 currentMap;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private int currentStageIndex;
    [SerializeField] private int currentStageSegmentIndex;
    [SerializeField] private List<string> finishedStoryEvents = new List<string>();

    public int CurrentStageSegmentIndex => currentStageSegmentIndex;
    public bool IsFinalStage => currentStageIndex == currentAdventure.Adventure.DynamicStages.Length - 1;
    public bool IsLastSegmentOfStage => currentMap.CurrentMapNode.Type == MapNodeType.Boss && currentStageSegmentIndex > 0;
    public bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    public int PartyCardCycles => currentAdventure.Adventure.BaseNumberOfCardCycles;
    public string[] FinishedStoryEvents => finishedStoryEvents.ToArray();
    public int Stage => currentStageIndex + 1;

    public DynamicStage CurrentStage
    {
        get { 
            if (currentStageIndex < 0 || currentStageIndex >= currentAdventure.Adventure.DynamicStages.Length)
                Log.Error($"Adventure Stage is illegal. {this}");
            return currentAdventure.Adventure.DynamicStages[currentStageIndex]; 
        }
    }
    
    public int CurrentPowerLevel => CurrentStage.GetPowerLevel(((float)currentStageSegmentIndex + 1) / CurrentStage.SegmentCount);
    public int CurrentElitePowerLevel => CurrentStage.GetElitePowerLevel(((float)currentStageSegmentIndex + 1) / CurrentStage.SegmentCount);
    private bool HasBegun => currentStageIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && currentStageSegmentIndex == CurrentStage.SegmentCount - 1;

    public void Init()
    {
        Reset();
        Log.Info($"Init Adventure. {this}");
    }

    public void Init(Adventure adventure)
    {
        currentAdventure.Adventure = adventure;
        Init();
        Log.Info($"Is advancing the adventure. {this}");
        Advance();
    }
    
    public override string ToString() =>
        $"Adventure: {currentAdventure.name}. Stage: {currentStageIndex}. StageSegment: {currentStageSegmentIndex}";

    public void InitIfNeeded()
    {
        if (HasBegun) return;
        
        Log.Info($"Is advancing the adventure. {this}");
        Advance();
    }
    
    public void Reset()
    {
        currentStageIndex = -1;
        currentStageSegmentIndex = -1;
        finishedStoryEvents.Clear();
    }

    public void Advance()
    {
        if (!HasBegun || CurrentStageIsFinished)
        {
            AdvanceStage();
        }
        currentStageSegmentIndex++;
        Log.Info(ToString());
    }

    private void AdvanceStage()
    {
        if (!IsFinalStage)
        {
            currentStageIndex++;
            currentStageSegmentIndex = -1;
            currentMap.SetMap(CurrentStage.Map);
        } else
        {
            Log.Info("Can't advance: is final stage");
        } 
    }

    public void RecordEncounteredStoryEvent(StoryEvent e) => finishedStoryEvents.Add(e.name);
}