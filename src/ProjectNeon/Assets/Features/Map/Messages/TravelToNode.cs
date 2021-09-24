using System;
using UnityEngine;

public class TravelToNode
{
    public Action<Transform> OnMidPointArrive;
    public Action<Transform> OnArrive;
    public Vector2 Position;
    [Obsolete]
    public GameObject Node;
    [Obsolete]
    public string NodeId;
    public bool TravelInstantly;
}