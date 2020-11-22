using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgress")]
public class AdventureProgress : ScriptableObject
{
    [SerializeField] private CurrentGameMap2 currentMap;
    [SerializeField] private Adventure currentAdventure;
    [SerializeField] private int currentStageIndex;
    [SerializeField] private int currentStageSegmentIndex;

    public Adventure Adventure => currentAdventure;
    public int CurrentStageSegmentIndex => currentStageSegmentIndex;
    public bool IsFinalStage => currentStageIndex == currentAdventure.Stages.Length - 1;
    public bool IsLastSegmentOfStage => currentStageSegmentIndex == CurrentStage.SegmentCount - 1;
    public bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    public int PartyCardCycles => currentAdventure.BaseNumberOfCardCycles;
    public StageBuilder CurrentStage
    {
        get { 
            if (currentStageIndex < 0 || currentStageIndex >= currentAdventure.Stages.Length)
                Log.Error($"Adventure Stage is illegal. {this}");
            return currentAdventure.Stages[currentStageIndex]; 
        }
    }
    
    public int CurrentPowerLevel => CurrentStage.GetPowerLevel(((float)currentStageSegmentIndex + 1) / CurrentStage.SegmentCount);
    private bool HasBegun => currentStageIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && currentStageSegmentIndex == CurrentStage.SegmentCount - 1;

    public void Init(Adventure a)
    {
        currentAdventure = a;
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
        if (currentAdventure.Stages.Length < 1)
            Log.Error("The adventure must have a least one stage!");
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
