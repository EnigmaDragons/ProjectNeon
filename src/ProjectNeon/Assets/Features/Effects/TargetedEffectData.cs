using System;

[Serializable]
public class TargetedEffectData
{
    public Scope Scope = Scope.All;
    public Group Group = Group.All;
    public EffectData EffectData;
}
