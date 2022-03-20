using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicPatientUIV5 : OnMessage<UpdateClinic, HeroStateChanged, PartyAdventureStateChanged>
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private HeroHpPresenter hpPresenter;
    [SerializeField] private Button healToFullButton;
    [SerializeField] private Button viewHeroDetailButton;
    [SerializeField] private TextMeshProUGUI healToFullCostLabel;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject injuriesParent;
    [SerializeField] private HealInjuryButton healInjuryButtonPrototype;
    
    private Hero _hero;
    private int _fullHealCost;
    private int _injuryHealCost;
    
    private void Awake()
    {
        healToFullButton.onClick.AddListener(HealHeroToFull);
        viewHeroDetailButton.onClick.AddListener(ViewHeroDetail);
        injuriesParent.DestroyAllChildren();
    }

    private void ViewHeroDetail() => Message.Publish(new ShowHeroDetailsView(_hero));
    
    public ClinicPatientUIV5 Initialized(Hero h)
    {
        _hero = h;
        nameLabel.text = h.DisplayName;
        hpPresenter.Init(h);
        UpdateCosts();
        UpdateButtons();
        return this;
    }
    
    protected override void Execute(UpdateClinic msg)
    {
        UpdateCosts();
        UpdateButtons();
    }

    private void UpdateCosts()
    {
        _fullHealCost = 1;
        _injuryHealCost = 1;
        healToFullCostLabel.text = _fullHealCost.ToString();
    }

    protected override void Execute(HeroStateChanged msg)
    {
        if (msg.Hero == _hero)
            UpdateButtons();
    }

    protected override void Execute(PartyAdventureStateChanged msg) => UpdateButtons();

    private void UpdateButtons()
    {
        healToFullButton.gameObject.SetActive(party.ClinicVouchers >= _fullHealCost && _hero.CurrentHp < _hero.Stats.MaxHp());
        injuriesParent.DestroyAllChildren();
        _hero.Health.InjuryNames.ForEach(x => Instantiate(healInjuryButtonPrototype, injuriesParent.transform)
            .Init(x, _injuryHealCost, party.ClinicVouchers >= _injuryHealCost ? (Action)(() => HealInjury(x)) : () => { }));
    }
    
    private void HealHeroToFull()
    {
        party.HealHeroToFull(_hero.Character);
        party.UpdateClinicVouchersBy(-1);
        Message.Publish(new UpdateClinic());
    }

    private void HealInjury(string injuryName)
    {
        _hero.HealInjuryByName(injuryName);
        party.UpdateCreditsBy(-_injuryHealCost);
        Message.Publish(new UpdateClinic());
    }
}
