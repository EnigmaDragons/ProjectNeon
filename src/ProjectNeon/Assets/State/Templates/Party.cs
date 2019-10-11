using UnityEngine;

public sealed class Party : ScriptableObject
{
    [SerializeField] private Hero heroOne;
    [SerializeField] private Hero heroTwo;
    [SerializeField] private Hero heroThree;

    public Hero[] Heroes => new []{ heroOne, heroTwo, heroThree };

    public Party Initialized(Hero one, Hero two, Hero three)
    {
        heroOne = one;
        heroTwo = two;
        heroThree = three;
        return this;
    }
}
