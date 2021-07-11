using System;
using UnityEngine;

[Serializable]
public class CorpAffinityLevelLines
{
    [SerializeField] private CorpAffinityStrength affinityStrength;
    [SerializeField, TextArea(1, 5)] private string[] lines = new string[0];

    public CorpAffinityStrength AffinityStrength => affinityStrength;
    public string[] Lines => lines;
}
