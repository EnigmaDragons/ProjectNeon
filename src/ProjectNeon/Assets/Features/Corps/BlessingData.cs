using System;

[Serializable]
public class BlessingData
{
    public string Name;
    public string Description;
    public bool IsSingleTarget;
    public StatType StatRequirement;
    public int RequirementThreshold;
    public EffectData Effect;
}