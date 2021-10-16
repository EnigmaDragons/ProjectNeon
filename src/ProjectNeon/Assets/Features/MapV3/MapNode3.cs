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

    public string GetMetricDescription()
        => ($"{Type} " + (string.IsNullOrWhiteSpace(Corp) ? "" : Corp)).Trim();
}