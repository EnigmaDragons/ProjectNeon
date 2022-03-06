using System;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentCutscene")]
public class CurrentCutscene : ScriptableObject
{
    [SerializeField] private Cutscene cutscene;

    public Maybe<Action> OnCutsceneFinishedAction { get; private set; } = Maybe<Action>.Missing();
    
    public Cutscene Current => cutscene;
    private IndexSelector<CutsceneSegmentData> _segments;
    public bool IsOnFinalSegment => _segments.IsLastItem;

    public void Init(Cutscene c, Maybe<Action> onCutsceneFinished)
    {
        cutscene = c;
        _segments = new IndexSelector<CutsceneSegmentData>(cutscene.Segments);
        OnCutsceneFinishedAction = onCutsceneFinished;
    }

    public CutsceneSegment MoveToNextSegment()
    {
        return AllCutsceneSegments.Create(_segments.MoveNextWithoutLooping());
    }
}
