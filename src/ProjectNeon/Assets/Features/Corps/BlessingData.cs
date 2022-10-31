using System;

[Serializable]
public class BlessingData
{
    public int Id;
    public string Name;
    public string Description;
    public bool IsSingleTarget;
    public StatType StatRequirement;
    public int RequirementThreshold;
    public EffectData Effect;
    public string NameTerm => $"Clinics/Blessing{Id}Name";
    public string DescriptionTerm => $"Clinics/Blessing{Id}Description";
}