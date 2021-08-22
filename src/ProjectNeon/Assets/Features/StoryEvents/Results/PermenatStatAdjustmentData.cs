using System;
using UnityEngine;

[Serializable]
public class PermenatStatAdjustmentData
{
    [SerializeField] private StatType stat;
    [SerializeField] private int amount;

    public StatType Stat => stat;
    public int Amount => amount;
}