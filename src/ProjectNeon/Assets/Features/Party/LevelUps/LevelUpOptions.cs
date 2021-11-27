using UnityEngine;

public abstract class LevelUpOptions : ScriptableObject
{
    public abstract string ChoiceDescription { get; }
    public abstract LevelUpOption[] Generate(Hero h);
}
