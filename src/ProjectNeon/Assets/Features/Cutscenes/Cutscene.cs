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
    public string[] SkipTrueStates => skipTrueStates.Select(x => x.Value).ToArray();
    public string[] SkipFalseStates => skipFalseStates.Select(x => x.Value).ToArray();
    public CutsceneSegmentData[] Segments => segments.ToArray();
}
