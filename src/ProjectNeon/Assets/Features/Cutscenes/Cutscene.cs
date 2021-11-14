using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Cutscene")]
public class Cutscene : ScriptableObject
{
    [SerializeField] private CutsceneSetting setting;
    [SerializeField] private CutsceneSegment[] segments;

    public CutsceneSetting Setting => setting;
    public CutsceneSegment[] Segments => segments.ToArray();
}
