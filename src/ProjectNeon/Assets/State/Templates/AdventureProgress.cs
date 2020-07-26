using System;
using UnityEngine;

class AdventureProgress : ScriptableObject
{
    [SerializeField] private Adventure currentAdventure;
    [SerializeField] private int currentStageIndex;
    [SerializeField] private int currentStageSegmentIndex;

    public int CurrentStageSegmentIndex => currentStageSegmentIndex;
    public bool IsFinalStage => currentStageIndex == currentAdventure.Stages.Length - 1;
    public bool IsFinalStageSegment => IsFinalStage && currentStageSegmentIndex == CurrentStage.Segments.Length - 1;
    public Stage CurrentStage
    {
        get { if (currentStageIndex < 0 || currentStageIndex >= currentAdventure.Stages.Length)
            Debug.LogError($"Adventure Stage is illegal. {this}");
            return currentAdventure.Stages[currentStageIndex]; }
    }

    public StageSegment CurrentStageSegment => CurrentStage.Segments[currentStageSegmentIndex];
    public bool HasStageBegun => currentStageSegmentIndex > -1;

    private bool HasBegun => currentStageIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && currentStageSegmentIndex == CurrentStage.Segments.Length - 1;

    public override string ToString() =>
        $"Adventure: {currentAdventure.name}. Stage: {currentStageIndex}. StageSegment: {currentStageSegmentIndex}";

    public void InitIfNeeded()
    {
        if (HasBegun) return;
        
        Debug.Log($"Is advancing the adventure. {this}");
        Advance();
    }
    
    public void Reset()
    {
        currentStageIndex = -1;
        currentStageSegmentIndex = -1;
        if (currentAdventure.Stages.Length < 1)
            Debug.LogError("The adventure must have a least one stage!");
    }

    public StageSegment Advance()
    {
        if (!HasBegun || CurrentStageIsFinished)
        {
            AdvanceStage();
        }

        if (currentStageSegmentIndex >= CurrentStage.Segments.Length)
        {
            Debug.LogError("Why the f**k are we advancing out of bounds?");
            Debug.Log(this);
            return CurrentStageSegment;
        }
        currentStageSegmentIndex++;
        Debug.Log(ToString());
        return CurrentStageSegment;
    }

    private void AdvanceStage()
    {
        if (!IsFinalStage)
        {
            currentStageIndex++;
            currentStageSegmentIndex = -1;
        } else
        {
            Debug.Log("Can't advance: is final stage");
        } 
    }
}
