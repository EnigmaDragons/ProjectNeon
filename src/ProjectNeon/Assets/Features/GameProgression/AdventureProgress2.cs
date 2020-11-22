using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgress2")]
public class AdventureProgress2 : ScriptableObject
{
    [SerializeField] private CurrentGameMap2 currentMap;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private int currentStageIndex;
    [SerializeField] private int currentStageSegmentIndex;

    public int CurrentStageSegmentIndex => currentStageSegmentIndex;
    public bool IsFinalStage => currentStageIndex == currentAdventure.Adventure.Stages.Length - 1;
    public bool IsLastSegmentOfStage => currentStageSegmentIndex == CurrentStage.SegmentCount - 1;
    public bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    public int PartyCardCycles => currentAdventure.Adventure.BaseNumberOfCardCycles;
    public DynamicStage CurrentStage
    {
        get { 
            if (currentStageIndex < 0 || currentStageIndex >= currentAdventure.Adventure.Stages.Length)
                Log.Error($"Adventure Stage is illegal. {this}");
            return currentAdventure.Adventure.DynamicStages[currentStageIndex]; 
        }
    }
    
    public int CurrentPowerLevel => CurrentStage.GetPowerLevel(((float)currentStageSegmentIndex + 1) / CurrentStage.SegmentCount);
    private bool HasBegun => currentStageIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && currentStageSegmentIndex == CurrentStage.SegmentCount - 1;

    public void Init()
    {
        Reset();
        Log.Info($"Init Adventure. {this}");
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
}