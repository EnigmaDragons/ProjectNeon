using UnityEngine;

public abstract class HeroLevelUpOption : ScriptableObject
{
    protected abstract string Description { get; }
    protected abstract void Apply(Hero h);
}
