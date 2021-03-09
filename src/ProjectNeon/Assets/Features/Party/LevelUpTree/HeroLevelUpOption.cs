using UnityEngine;

public abstract class HeroLevelUpOption : ScriptableObject
{
    public abstract string IconName { get; }
    public abstract string Description { get; }
    public abstract void Apply(Hero h);
}
