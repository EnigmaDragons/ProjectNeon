using System;
using UnityEngine;

[Serializable]
public class HeatUpEventV0
{
    [Range(0, 1)] public float ProgressThreshold;
    [TextArea(2, 4)] public string InfoText;
    public HeatUpEventMapNode[] MapNodes;
}
