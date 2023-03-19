using UnityEngine;

public abstract class LevelUpOptions : ScriptableObject
{
    public abstract string ChoiceDescriptionTerm { get; }
    public abstract LevelUpOption[] Generate(Hero h);
}
