using System.Collections.Generic;
using UnityEngine;

public sealed class Party : ScriptableObject
{
    [SerializeField] private BaseHero heroOne;
    [SerializeField] private BaseHero heroTwo;
    [SerializeField] private BaseHero heroThree;
    
    private List<BaseHero> _heroes = new List<BaseHero>();
    
    public BaseHero[] Heroes => _heroes.ToArray();

    public Party Initialized(BaseHero one, BaseHero two, BaseHero three)
    {
        heroOne = one;
        heroTwo = two;
        heroThree = three;
        _heroes = new List<BaseHero>();
        if (heroOne != null && !heroOne.name.Equals("NoHero"))
            _heroes.Add(heroOne);
        if (heroTwo != null && !heroTwo.name.Equals("NoHero"))
            _heroes.Add(heroTwo);
        if (heroThree != null && !heroThree.name.Equals("NoHero"))
            _heroes.Add(heroThree);
        return this;
    }
}
