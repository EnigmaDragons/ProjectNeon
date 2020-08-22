using System;

[Serializable]
public class EquipmentStatModifier
{
    public StatMathOperator ModifierType;
    public float Amount;
    public string StatType;

    public string Describe() => $"{ModifierType.Describe()}{Amount} {StatType}";
}
