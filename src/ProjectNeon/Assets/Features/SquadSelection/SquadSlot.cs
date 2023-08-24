using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquadSlot : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private HeroPool heroPool;
    [SerializeField] private HeroDisplayPresenter presenter;

    [ReadOnly, SerializeField] private BaseHero current;
    private int _index;
    private BaseHero[] _bannedHeroes;
    
    public void Init(int slotIndex, BaseHero[] bannedHeroes)
    {
        _index = slotIndex;
        _bannedHeroes = bannedHeroes;
        SelectNextHero();
        SelectPreviousHero();
        gameObject.SetActive(true);
    }

    public void Randomize(DeterministicRng rng) =>
        Enumerable.Range(0, rng.Int(AvailableHeroes.Count())).ForEach(_ => SelectNextHero());
    
    public void SelectNextHero()
    {
        if (current != null)
            heroPool.Unselect(_index, current);
        SelectHero(_index, AvailableHeroes.Concat(AvailableHeroes)
            .SkipWhile(x => current != null && x != current)
            .Skip(1)
            .First(), false);
    }

    public void SelectPreviousHero()
    {
        if (current != null)
            heroPool.Unselect(_index, current);
        SelectHero(_index, AvailableHeroes.Concat(AvailableHeroes).Reverse().SkipWhile(x => current != null && x != current).Skip(1).First(), false);
    }

    public void SelectRequiredHero(BaseHero c)
    {
        SelectHero(_index, c, true);
    }

    private void SelectHero(int index, BaseHero c, bool required)
    {
        current = c;
        heroPool.Select(index, c);
        presenter.Init(c, !required, SelectNextHero);
        presenter.SetControlText("Menu/Change");
    }

    private IEnumerable<BaseHero> AvailableHeroes => heroPool.AvailableHeroes.Except(_bannedHeroes);
    public string[] GetLocalizeTerms() => new [] {"Menu/Change"};
}
