using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PartyDetailsUI : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private HeroDetailsPanel panelPrototype;
    [SerializeField] private Transform parent;
        
    private readonly List<HeroDetailsPanel> _panels = new List<HeroDetailsPanel>();

    protected override void AfterEnable()
    {
        if (_panels.None())
            Enumerable.Range(0, 3).ForEach(_ => _panels.Add(Instantiate(panelPrototype, parent)));
        UpdateUi();
    }

    private void UpdateUi()
    {
        for (var i = 0; i < party.BaseHeroes.Length; i++)
            _panels[i].Initialized(party.Heroes[i], party.Heroes[i].AsMember(i));
    }

    protected override void Execute(PartyAdventureStateChanged msg) => UpdateUi();
}
