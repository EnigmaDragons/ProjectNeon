using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Cutscene")]
public class Cutscene : ScriptableObject, ILocalizeTerms
{
    [SerializeField] public int id;
    [SerializeField] private bool isObsolete = false;
    [SerializeField] private bool isPrimaryCutscene;
    [SerializeField] private CutsceneSetting setting;
    [SerializeField] private StringReference[] skipTrueStates;
    [SerializeField] private StringReference[] skipFalseStates;
    [SerializeField] private CutsceneSegmentData[] segments;

    public bool IsObsolete => isObsolete;
    public bool IsPrimaryCutscene => isPrimaryCutscene;
    public CutsceneSetting Setting => setting;
    public CutsceneSegmentData[] Segments => segments.ToArray();

    public void MarkSkipped(AdventureProgressBase a)
    {
        skipTrueStates.ForEach(x => a.SetStoryState(x, true));
        skipFalseStates.ForEach(x => a.SetStoryState(x, false));
    }

    public string[] GetLocalizeTerms()
    {
        return segments.Select(x => x.Term).ToArray();
    }
}
