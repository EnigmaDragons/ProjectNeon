using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Stage")]
public class Stage : ScriptableObject
{
    [SerializeField] private StageSegment[] segments;

    public StageSegment[] Segments => segments.ToArray();
}
