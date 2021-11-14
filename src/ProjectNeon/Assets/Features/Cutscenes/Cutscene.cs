using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Cutscene")]
public class Cutscene : ScriptableObject
{
    [SerializeField] private CutsceneSetting setting;
    [SerializeField] private CutsceneSegmentData[] segments;

    public CutsceneSetting Setting => setting;
    public CutsceneSegmentData[] Segments => segments.ToArray();
}
