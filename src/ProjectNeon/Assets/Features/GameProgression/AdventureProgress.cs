﻿using System;
using UnityEngine;

[Obsolete("MapView1")]
[CreateAssetMenu(menuName = "GameState/AdventureProgress")]
public class AdventureProgress : ScriptableObject
{
    [SerializeField] private CurrentGameMap currentMap;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private int currentStageIndex;
    [SerializeField] private int currentStageSegmentIndex;
    
    public int CurrentStageSegmentIndex => currentStageSegmentIndex;
    public bool IsFinalStage => currentStageIndex == currentAdventure.Adventure.Stages.Length - 1;
    public bool IsLastSegmentOfStage => currentStageSegmentIndex == CurrentStage.Segments.Length - 1;
    public bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    public Stage CurrentStage
    {
        get { 
            if (currentStageIndex < 0 || currentStageIndex >= currentAdventure.Adventure.Stages.Length)
                Log.Error($"Adventure Stage is illegal. {this}");
            return currentAdventure.Adventure.Stages[currentStageIndex];
        }
    }

    public StageSegment CurrentStageSegment => CurrentStage.Segments[currentStageSegmentIndex];
    public bool HasStageBegun => currentStageSegmentIndex > -1;

    private bool HasBegun => currentStageIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && currentStageSegmentIndex == CurrentStage.Segments.Length - 1;

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
        if (currentAdventure.Adventure.Stages.Length < 1)
            Log.Error("The adventure must have a least one stage!");
    }

    public StageSegment Advance()
    {
        if (!HasBegun || CurrentStageIsFinished)
        {
            AdvanceStage();
        }

        if (currentStageSegmentIndex >= CurrentStage.Segments.Length)
        {
            Log.Error("Why the f**k are we advancing out of bounds?");
            Log.Info(this);
            return CurrentStageSegment;
        }
        currentStageSegmentIndex++;
        Log.Info(ToString());
        currentMap.SetMap(CurrentStage.Map);
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
            Log.Info("Can't advance: is final stage");
        } 
    }
}
