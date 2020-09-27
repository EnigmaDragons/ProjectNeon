using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/HeroPool")]
public sealed class HeroPool : ScriptableObject
{
    [SerializeField] private Library library;
    [SerializeField] private BaseHero[] selected;

    public IEnumerable<BaseHero> AvailableHeroes => library.UnlockedHeroes.Except(selected).ToArray();
    public BaseHero[] SelectedHeroes => selected.ToArray();

    public void ClearSelections() => selected = new BaseHero[3];
    public void Select(int index, BaseHero c) => selected[index] = c;
    public void Unselect(int index, BaseHero c) => selected[index] = null;
}
