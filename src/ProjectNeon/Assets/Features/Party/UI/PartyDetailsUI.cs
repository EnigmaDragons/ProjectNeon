using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class PartyDetailsUI : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private HeroDetailsPanel panelPrototype;
    [SerializeField] private Transform parent;
    [SerializeField] private Button doneButton;
    [SerializeField] private Button nextButton;
    
    private readonly List<HeroDetailsPanel> _panels = new List<HeroDetailsPanel>();
    
    public void UseNextButtonOneTime()
    {
        doneButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
    }

    private void Awake()
    {
        parent.DestroyAllChildren();
    }
    
    protected override void AfterEnable()
    {
        if (_panels.None())
            Enumerable.Range(0, party.BaseHeroes.Length).ForEach(_ => _panels.Add(Instantiate(panelPrototype, parent)));
        UpdateUi();
        doneButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(false);
    }

    private void UpdateUi()
    {
        for (var i = 0; i < party.BaseHeroes.Length; i++)
            _panels[i].Initialized(party.Heroes[i], party.Heroes[i].AsMember(i), canInteractWithEquipment: true);
    }

    protected override void Execute(PartyAdventureStateChanged msg) => UpdateUi();
}
