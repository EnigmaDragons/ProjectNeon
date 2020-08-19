using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class HeroPool : ScriptableObject
{
    [SerializeField] private Library library;
    [SerializeField] private BaseHero[] selected;

    public IEnumerable<BaseHero> AvailableHeroes => library.UnlockedHeroes.Except(selected).ToArray();
    public BaseHero[] SelectedHeroes => selected.ToArray();

    public void ClearSelections() => selected = Array.Empty<BaseHero>();
    public void Select(BaseHero c) => selected = selected.Concat(c).ToArray();
    public void Unselect(BaseHero c) => selected = selected.Except(c).ToArray();
}
