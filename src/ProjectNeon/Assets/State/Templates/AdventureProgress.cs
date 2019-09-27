using UnityEngine;

class AdventureProgress : ScriptableObject
{
    [SerializeField] private Adventure currentAdventure;
    [SerializeField] private int currentStageIndex;
    [SerializeField] private int currentStageSegmentIndex;

    public bool IsFinalStage => currentStageIndex == currentAdventure.Stages.Length - 1;
    public bool IsFinalStageSegment => IsFinalStage && currentStageSegmentIndex == CurrentStage.Segments.Length - 1;
    public Stage CurrentStage => currentAdventure.Stages[currentStageIndex];
    public StageSegment CurrentStageSegment => CurrentStage.Segments[currentStageSegmentIndex];

    private bool HasBegun => currentStageIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && currentStageSegmentIndex == CurrentStage.Segments.Length - 1;

    public void Reset()
    {
        currentStageIndex = -1;
    }

    public StageSegment Advance()
    {
        if (!HasBegun || CurrentStageIsFinished)
            AdvanceStage();

        currentStageSegmentIndex++;
        return CurrentStageSegment;
    }

    private void AdvanceStage()
    {
        if (!IsFinalStage)
        {
            currentStageIndex++;
            currentStageSegmentIndex = -1;
        } 
    }
}
