using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Cutscene")]
public class Cutscene : ScriptableObject
{
    [SerializeField] private CutsceneSetting setting;
    [SerializeField] private StringReference[] skipTrueStates;
    [SerializeField] private StringReference[] skipFalseStates;
    [SerializeField] private CutsceneSegmentData[] segments;

    public CutsceneSetting Setting => setting;
    public CutsceneSegmentData[] Segments => segments.ToArray();

    public void MarkSkipped(AdventureProgressBase a)
    {
        skipTrueStates.ForEach(x => a.SetStoryState(x, true));
        skipFalseStates.ForEach(x => a.SetStoryState(x, false));
    }
}
