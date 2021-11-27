using UnityEngine;

public abstract class LevelUpOptions : ScriptableObject
{
    public abstract LevelUpOption[] Generate(Hero h);
}
