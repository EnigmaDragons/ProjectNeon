using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Stage Segments")]
public class AllStageSegments : ScriptableObject
{
    private Dictionary<int, StageSegment> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public StageSegment[] Stages; //Unity Collection Readonly

    public Dictionary<int, StageSegment> GetMap() => _map ??= Stages.ToDictionary(x => x.Id, x => x);
    public Maybe<StageSegment> GetStageSegmentById(int id) => GetMap().ValueOrMaybe(id);
}