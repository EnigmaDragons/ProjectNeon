using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Stage")]
public class Stage : ScriptableObject
{
    [SerializeField] private GameMap map;
    [SerializeField] private StorySetting setting;
    [SerializeField] private StageSegment[] segments;

    public GameMap Map => map;
    public StorySetting Setting => setting;
    public StageSegment[] Segments => segments.ToArray();
}
