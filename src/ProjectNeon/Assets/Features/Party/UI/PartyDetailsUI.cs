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
        for (var i = 0; i < party.BaseHeroes.Length; i++)
            _panels[i].Initialized(party.Heroes[i], party.Heroes[i].AsMember(i));
    }
}
