using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/Party")]
public sealed class Party : ScriptableObject
{
    [SerializeField] private BaseHero[] heroes;
    
    private List<BaseHero> _heroes = new List<BaseHero>();
    
    public BaseHero[] Heroes => heroes;

    public Party Initialized(BaseHero one, BaseHero two, BaseHero three)
    {
        _heroes = new List<BaseHero>();
        Add(one);
        Add(two);
        Add(three);
        return this;
    }

    public void Remove(BaseHero hero)
    {
        _heroes.Remove(hero);
        heroes = _heroes.ToArray();
    }

    public void Add(BaseHero hero)
    {
        if (_heroes.Count == 3)
        {
            Log.Error("Developer Error - Only 3 Heroes Allowed in Party. Cannot add another");
            return;
        }
        
        if (hero != null && !hero.name.Equals("NoHero"))
            _heroes.Add(hero);

        heroes = _heroes.ToArray();
    }
}
