using System;
using UnityEngine;

[Serializable]
public class CorpCostModifier
{
    public string Corp { get; set; }
    public bool AppliesToEquipmentShop { get; set; }
    public bool AppliesToClinic { get; set; }
    public float CostPercentageModifier { get; set; }
}