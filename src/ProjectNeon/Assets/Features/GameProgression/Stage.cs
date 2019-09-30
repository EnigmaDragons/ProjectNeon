using System.Linq;
using UnityEngine;

public class Stage : ScriptableObject
{
    [SerializeField] private StageSegment[] segments;

    public StageSegment[] Segments => segments.ToArray();
}
