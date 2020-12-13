using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicPatientUI : OnMessage<UpdateClinicServiceRates, HeroStateChanged>
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private HeroHpPresenter hpPresenter;
    [SerializeField] private Button healToFullButton;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject injuriesParent;
    [SerializeField] private TextCommandButton injuryButtonPrototype;

    private int _serviceCost;
    private Hero _hero;

    private void Awake()
    {
        healToFullButton.onClick.AddListener(HealHeroToFull);
        injuriesParent.DestroyAllChildren();
    }
    
    public ClinicPatientUI Initialized(Hero h)
    {
        _hero = h;
        nameLabel.text = h.Name;
        hpPresenter.Init(h);
        UpdateButtons();
        return this;
    }
    
    protected override void Execute(UpdateClinicServiceRates msg)
    {
        _serviceCost = msg.Cost;
        UpdateButtons();
    }

    protected override void Execute(HeroStateChanged msg)
    {
        if (msg.Hero == _hero)
            UpdateButtons();
    }

    private void HealHeroToFull()
    {
        party.UpdateCreditsBy(-_serviceCost);
        party.HealHeroToFull(_hero.Character);
        healToFullButton.gameObject.SetActive(false);
    }
    
    private void UpdateButtons()
    {
        var canAfford = party.Credits >= _serviceCost;
        healToFullButton.gameObject.SetActive(canAfford && _hero.CurrentHp < _hero.Stats.MaxHp());
        injuriesParent.DestroyAllChildren();
        _hero.Health.InjuryNames.ForEach(x => Instantiate(injuryButtonPrototype, injuriesParent.transform)
            .Init(x, canAfford ? (Action)(() => _hero.HealInjuryByName(x)) : () => { }));
    }
}
