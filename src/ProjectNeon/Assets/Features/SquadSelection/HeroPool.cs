using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class HeroPool : ScriptableObject
{
    [SerializeField] private Library library;
    [SerializeField] private Hero[] selected;

    public IEnumerable<Hero> AvailableHeroes => library.UnlockedHeroes.Except(selected).ToArray();
    public Hero[] SelectedHeroes => selected.ToArray();

    public void ClearSelections() => selected = Array.Empty<Hero>();
    public void Select(Hero c) => selected = selected.Concat(c).ToArray();
    public void Unselect(Hero c) => selected = selected.Except(c).ToArray();
}
