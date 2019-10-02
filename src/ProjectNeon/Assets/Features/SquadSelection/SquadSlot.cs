using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquadSlot : MonoBehaviour
{
    [SerializeField] private Hero defaultHero;
    [SerializeField] private Hero noHero;
    [SerializeField] private HeroPool heroPool;
    [SerializeField] private HeroDisplayPresenter presenter;
    
    [ReadOnly] [SerializeField] private Hero current;

    private void Awake()
    {
        heroPool.ClearSelections();
    }

    private void Start()
    {
        if (heroPool.AvailableHeroes.None())
            throw new InvalidOperationException("No Available Heroes");
        SelectHero(defaultHero);
    }
    
    public void SelectNextHero()
    {
        heroPool.Unselect(current);
        SelectHero(AvailableHeroesIncludingNone.SkipWhile(x => x != current).Skip(1).First());
    }

    public void SelectPreviousHero()
    {
        heroPool.Unselect(current);
        SelectHero(AvailableHeroesIncludingNone.Reverse().SkipWhile(x => x != current).Skip(1).First());
    }

    private void SelectHero(Hero c)
    {
        current = c;
        heroPool.Select(c);
        presenter.Select(c);
    }

    private IEnumerable<Hero> AvailableHeroesIncludingNone => heroPool.AvailableHeroes.WrappedWith(noHero);
}
