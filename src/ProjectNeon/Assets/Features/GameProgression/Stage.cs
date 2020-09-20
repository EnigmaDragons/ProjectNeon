using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Stage")]
public class Stage : ScriptableObject
{
    [SerializeField] private GameMap map;
    [SerializeField] private StageSegment[] segments;

    public GameMap Map => map;
    public StageSegment[] Segments => segments.ToArray();
}
