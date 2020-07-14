using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquadSlot : MonoBehaviour
{
    [SerializeField] private HeroPool heroPool;
    [SerializeField] private HeroDisplayPresenter presenter;
    
    [ReadOnly, SerializeField] private Hero current;

    public void SelectNextHero()
    {
        if (current != null)
            heroPool.Unselect(current);
        SelectHero(AvailableHeroes.SkipWhile(x => current != null && x != current).Skip(1).First());
    }

    public void SelectPreviousHero()
    {
        if (current != null)
            heroPool.Unselect(current);
        SelectHero(AvailableHeroes.Reverse().SkipWhile(x => current != null && x != current).Skip(1).First());
    }

    private void SelectHero(Hero c)
    {
        current = c;
        heroPool.Select(c);
        presenter.Select(c);
    }

    private IEnumerable<Hero> AvailableHeroes => heroPool.AvailableHeroes;
}
