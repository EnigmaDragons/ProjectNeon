using System.Collections.Generic;
using UnityEngine;

public sealed class Party : ScriptableObject
{
    [SerializeField] private Hero heroOne;
    [SerializeField] private Hero heroTwo;
    [SerializeField] private Hero heroThree;
    
    private List<Hero> _heroes = new List<Hero>();
    
    public Hero[] Heroes => _heroes.ToArray();

    public Party Initialized(Hero one, Hero two, Hero three)
    {
        heroOne = one;
        heroTwo = two;
        heroThree = three;
        _heroes = new List<Hero>();
        if (heroOne != null && !heroOne.name.Equals("NoHero"))
            _heroes.Add(heroOne);
        if (heroTwo != null && !heroTwo.name.Equals("NoHero"))
            _heroes.Add(heroTwo);
        if (heroThree != null && !heroThree.name.Equals("NoHero"))
            _heroes.Add(heroThree);
        return this;
    }
}
