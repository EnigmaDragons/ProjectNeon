using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentCutscene")]
public class CurrentCutscene : ScriptableObject
{
    [SerializeField] private Cutscene cutscene;

    public Cutscene Current => cutscene;
    private IndexSelector<CutsceneSegmentData> _segments;
    public bool IsOnFinalSegment => _segments.IsLastItem;

    public void Init(Cutscene c)
    {
        cutscene = c;
        _segments = new IndexSelector<CutsceneSegmentData>(cutscene.Segments);
    }

    public CutsceneSegment MoveToNextSegment()
    {
        return AllCutsceneSegments.Create(_segments.MoveNextWithoutLooping());
    }
}
