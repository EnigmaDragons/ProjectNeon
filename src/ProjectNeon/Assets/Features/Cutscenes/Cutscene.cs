using System.Linq;
using UnityEngine;

public class Cutscene : ScriptableObject
{
    [SerializeField] private CutsceneSegment[] segments;

    public CutsceneSegment[] Segments => segments.ToArray();
}
