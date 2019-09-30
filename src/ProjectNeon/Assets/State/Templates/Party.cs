using UnityEngine;

public class Party : ScriptableObject
{
    // @todo #1:10min change this to a dynamic size array

    [SerializeField]
    public Hero heroOne;

    [SerializeField]
    public Hero heroTwo;

    [SerializeField]
    public Hero heroThree;

    [SerializeField] private IntVariable totalPowerLevel;

    public Party Initialized(Hero one, Hero two, Hero three)
    {
        heroOne = one;
        heroTwo = two;
        heroThree = three;
        return this;
    }
}
