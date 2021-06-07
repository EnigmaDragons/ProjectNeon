using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquadSlot : MonoBehaviour
{
    [SerializeField] private HeroPool heroPool;
    [SerializeField] private HeroDisplayPresenter presenter;
    [SerializeField] private GameObject[] controls;
    
    [ReadOnly, SerializeField] private BaseHero current;
    private int _index;
    private BaseHero[] _bannedHeroes;
    
    public void Init(int index, BaseHero[] bannedHeroes)
    {
        _index = index;
        _bannedHeroes = bannedHeroes;
        SelectNextHero();
        SelectPreviousHero();
    }
    
    public void SelectNextHero()
    {
        if (current != null)
            heroPool.Unselect(_index, current);
        SelectHero(_index, AvailableHeroes.Concat(AvailableHeroes)
            .SkipWhile(x => current != null && x != current)
            .Skip(1)
            .First());
    }

    public void SelectPreviousHero()
    {
        if (current != null)
            heroPool.Unselect(_index, current);
        SelectHero(_index, AvailableHeroes.Concat(AvailableHeroes).Reverse().SkipWhile(x => current != null && x != current).Skip(1).First());
    }

    public void SelectRequiredHero(BaseHero c)
    {
        SelectHero(_index, c);
        SetNoChoicesAvailable();
    }

    public void SetNoChoicesAvailable() => controls.ForEach(x => x.SetActive(false));
    
    private void SelectHero(int index, BaseHero c)
    {
        current = c;
        heroPool.Select(index, c);
        presenter.Select(c);
    }

    private IEnumerable<BaseHero> AvailableHeroes => heroPool.AvailableHeroes.Except(_bannedHeroes);
}
