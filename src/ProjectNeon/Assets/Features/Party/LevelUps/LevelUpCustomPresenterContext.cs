using UnityEngine;

public class LevelUpCustomPresenterContext
{
    public Transform Parent { get; }
    public Hero Hero { get; }
    public LevelUpOption Option { get; }
    public LevelUpOption[] AllOptions { get; }

    public LevelUpCustomPresenterContext(Transform parent, Hero hero, LevelUpOption option, LevelUpOption[] allOptions)
    {
        Parent = parent;
        Hero = hero;
        Option = option;
        AllOptions = allOptions;
    }
}
