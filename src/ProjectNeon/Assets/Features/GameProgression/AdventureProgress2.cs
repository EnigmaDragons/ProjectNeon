using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/AdventureProgress2")]
public class AdventureProgress2 : ScriptableObject
{
    [SerializeField] private CurrentGameMap3 currentMap3;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private int currentChapterIndex;
    [SerializeField] private List<string> finishedStoryEvents = new List<string>();

    public Adventure CurrentAdventure => currentAdventure.Adventure;
    public int CurrentAdventureId => currentAdventure.Adventure.Id;
    public int CurrentChapterNumber => currentChapterIndex + 1;
    public int CurrentChapterIndex => currentChapterIndex;
    public int CurrentStageSegmentIndex => currentMap3.Progress;
    public float ProgressToUnlockChapterBoss => CurrentStageSegmentIndex == 0 ? 0f : (float)CurrentStageSegmentIndex / CurrentChapter.SegmentCount;
    public bool IsFinalStage => currentChapterIndex == currentAdventure.Adventure.DynamicStages.Length - 1;
    public bool IsLastSegmentOfStage => currentMap3.CompletedNodes.Any() && currentMap3.CompletedNodes[currentMap3.CompletedNodes.Count - 1] == MapNodeType.Boss;
    public bool IsFinalStageSegment => IsFinalStage && IsLastSegmentOfStage;
    public string[] FinishedStoryEvents => finishedStoryEvents.ToArray();

    public DynamicStage CurrentChapter
    {
        get { 
            if (currentChapterIndex < 0 || currentChapterIndex >= currentAdventure.Adventure.DynamicStages.Length)
                Log.Error($"Adventure Stage is illegal. {this}");
            return currentAdventure.Adventure.DynamicStages[currentChapterIndex]; 
        }
    }
    
    public int CurrentPowerLevel => CurrentChapter.GetPowerLevel(((float)currentMap3.Progress + 1) / CurrentChapter.SegmentCount);
    public int CurrentElitePowerLevel => CurrentChapter.GetElitePowerLevel(((float)currentMap3.Progress + 1) / CurrentChapter.SegmentCount);
    private bool HasBegun => currentChapterIndex > -1;
    private bool CurrentStageIsFinished => HasBegun && currentMap3.Progress == CurrentChapter.SegmentCount;

    public void Init()
    {
        Reset();
        Log.Info($"Init Adventure. {this}");
    }

    public void Init(Adventure adventure, int chapterIndex, int stageSegmentIndex)
    {
        currentAdventure.Adventure = adventure;
        Init();
        Advance();
        currentChapterIndex = chapterIndex;
        Log.Info($"Init Adventure. {this}");
    }
    
    public override string ToString() =>
        $"Adventure: {currentAdventure.name}. Stage: {currentChapterIndex}. StageProgress: {currentMap3.Progress}";

    public void InitIfNeeded()
    {
        if (HasBegun) return;
        
        Log.Info($"Is advancing the adventure. {this}");
        Advance();
    }
    
    public void Reset()
    {
        currentChapterIndex = -1;
        finishedStoryEvents.Clear();
        Message.Publish(new AdventureProgressChanged());
    }

    public void Advance()
    {
        if (!HasBegun || CurrentStageIsFinished)
        {
            AdvanceStage();
        }
        Log.Info(ToString());
        Message.Publish(new AdventureProgressChanged());
    }

    private void AdvanceStage()
    {
        if (!IsFinalStage)
        {
            currentChapterIndex++;
            currentMap3.SetMap(CurrentChapter.Map);
        } else
        {
            Log.Info("Can't advance: is final stage");
        } 
    }

    public void RecordEncounteredStoryEvent(StoryEvent e) => finishedStoryEvents.Add(e.name);
}