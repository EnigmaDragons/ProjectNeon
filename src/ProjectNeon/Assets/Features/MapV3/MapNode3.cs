using System;
using UnityEngine;

[Serializable]
public class MapNode3
{
    public MapNodeType Type;
    public Vector2 Position;
    public bool PreventTravel;
    public bool HasEventEnroute;
    public int[] EnemyIds;
    public string Corp;
    
    public bool IsPlotNode;
    public int VisitedGlobalEffectId = -1;
    public int UnVisitedGlobalEffectId = -1;

    public string GetMetricDescription()
        => ($"{Type} " + (string.IsNullOrWhiteSpace(Corp) ? "" : Corp)).Trim();
}