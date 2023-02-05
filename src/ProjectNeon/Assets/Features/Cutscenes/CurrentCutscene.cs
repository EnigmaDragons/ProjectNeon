using System;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentCutscene")]
public class CurrentCutscene : ScriptableObject
{
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private Cutscene startBattleCutscene;
    [SerializeField] private bool startBattleCutsceneFinished;

    public Maybe<Action> OnCutsceneFinishedAction { get; private set; } = Maybe<Action>.Missing();
    
    public Cutscene Current => startBattleCutscene != null ? startBattleCutscene : cutscene;
    public Cutscene StartBattleCutscene => startBattleCutscene;
    private IndexSelector<CutsceneSegmentData> _segments;
    public bool IsOnFinalSegment => _segments.IsLastItem;
    public bool HasStartBattleCutscene => startBattleCutscene != null && !startBattleCutsceneFinished;

    public void Reset() => _segments = new IndexSelector<CutsceneSegmentData>(cutscene.Segments);
    
    public void Init(Cutscene c, Maybe<Action> onCutsceneFinished)
    {
        cutscene = c;
        startBattleCutscene = null;
        _segments = new IndexSelector<CutsceneSegmentData>(cutscene.Segments);
        OnCutsceneFinishedAction = onCutsceneFinished;
    }

    public void InitStartBattle(Cutscene c)
    {
        cutscene = null;
        startBattleCutscene = c;
        _segments = new IndexSelector<CutsceneSegmentData>(startBattleCutscene.Segments);
        startBattleCutsceneFinished = false;
    }

    public void FinishStartBattleCutscene()
    {
        startBattleCutsceneFinished = true;
        if (startBattleCutscene != null)
            startBattleCutscene.OnFinished();
        startBattleCutscene = null;
    }

    public CutsceneSegment MoveToNextSegment()
    {
        return AllCutsceneSegments.Create(_segments.MoveNextWithoutLooping());
    }
}
