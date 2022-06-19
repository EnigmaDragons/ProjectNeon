using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicPatientUIV5 : OnMessage<UpdateClinic, HeroStateChanged, PartyAdventureStateChanged, SetSuperFocusHealControl>
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private HeroHpPresenter hpPresenter;
    [SerializeField] private Button healToFullButton;
    [SerializeField] private GameObject healToFullSuperFocus;
    [SerializeField] private GameObject fullHealth;
    [SerializeField] private Button viewHeroDetailButton;
    [SerializeField] private TextMeshProUGUI healToFullCostLabel;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject injuriesParent;
    [SerializeField] private GameObject noInjuriesPrototype;
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
    protected override void Execute(SetSuperFocusHealControl msg) => healToFullSuperFocus.SetActive(msg.Enabled);

    private void UpdateButtons()
    {
        healToFullButton.gameObject.SetActive(party.ClinicVouchers >= _fullHealCost && _hero.CurrentHp < _hero.Stats.MaxHp());
        if (fullHealth != null)
            fullHealth.SetActive(_hero.CurrentHp >= _hero.Stats.MaxHp());
        injuriesParent.DestroyAllChildren();
        _hero.Health.InjuryNames.ForEach(x => Instantiate(healInjuryButtonPrototype, injuriesParent.transform)
            .Init(x, _injuryHealCost, party.ClinicVouchers >= _injuryHealCost ? (Action)(() => HealInjury(x)) : () => { }));
        if (noInjuriesPrototype != null && _hero.Health.InjuryNames.None())
            Instantiate(noInjuriesPrototype, injuriesParent.transform);
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
