using UnityEngine;

public class LevelUpCustomPresenterContext
{
    public Transform Parent { get; }
    public CustomLevelUpPresenters Presenters { get; }
    public Hero Hero { get; }
    public LevelUpOption Option { get; }
    public LevelUpOption[] AllOptions { get; }

    public LevelUpCustomPresenterContext(Transform parent, CustomLevelUpPresenters presenters, Hero hero, LevelUpOption option, LevelUpOption[] allOptions)
    {
        Parent = parent;
        Presenters = presenters;
        Hero = hero;
        Option = option;
        AllOptions = allOptions;
    }
}
