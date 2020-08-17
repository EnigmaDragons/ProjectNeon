using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PartyDetailsUI : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private HeroDetailsPanel panelPrototype;
    [SerializeField] private Transform parent;
        
    private readonly List<HeroDetailsPanel> _panels = new List<HeroDetailsPanel>();

    private void OnEnable()
    {
        if (_panels.None())
            Enumerable.Range(0, 3).ForEach(_ => _panels.Add(Instantiate(panelPrototype, parent)));
        for (var i = 0; i < party.Heroes.Length; i++)
        {
            var hero = party.Heroes[i];
            _panels[i].Initialized(hero, new Member(i, hero.Name, hero.Class.Name, TeamType.Party, hero.Stats, party.CurrentHpOf(hero)));
        }
    }
}
