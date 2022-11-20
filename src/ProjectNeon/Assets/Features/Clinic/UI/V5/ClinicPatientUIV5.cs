using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicPatientUIV5 : OnMessage<UpdateClinic, HeroStateChanged, PartyAdventureStateChanged, SetSuperFocusHealControl>
{
    [SerializeField] private Localize nameLocalize;
    [SerializeField] private HeroHpPresenter hpPresenter;
    [SerializeField] private Button healToFullButton;
    [SerializeField] private GameObject healToFullSuperFocus;
    [SerializeField] private GameObject fullHealth;
    [SerializeField] private Button viewHeroDetailButton;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI healToFullCostLabel;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject injuriesParent;
    [SerializeField] private GameObject noInjuriesPrototype;
    [SerializeField] private HealInjuryButton healInjuryButtonPrototype;
    
    private Hero _hero;
    private const int FullHealCost = 1;
    private const int InjuryHealCost = 1;
    
    private void Awake()
    {
        healToFullButton.onClick.AddListener(HealHeroToFull);
        viewHeroDetailButton.onClick.AddListener(ViewHeroDetail);
        injuriesParent.DestroyAllChildren();
    }

    private void ViewHeroDetail() => Message.Publish(new ShowHeroDetailsView(_hero, Maybe<Member>.Missing()));
    
    public ClinicPatientUIV5 Initialized(Hero h)
    {
        _hero = h;
        nameLocalize.SetTerm(h.NameTerm);
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
        healToFullCostLabel.text = FullHealCost.ToString();
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
        healToFullButton.gameObject.SetActive(party.ClinicVouchers >= FullHealCost && _hero.CurrentHp < _hero.Stats.MaxHp());
        if (fullHealth != null)
            fullHealth.SetActive(_hero.CurrentHp >= _hero.Stats.MaxHp());
        injuriesParent.DestroyAllChildren();
        _hero.Health.AllInjuries
            .DistinctBy(x => x.InjuryName)
            .ForEach(x => Instantiate(healInjuryButtonPrototype, injuriesParent.transform)
                .Init(x, InjuryHealCost, party.ClinicVouchers >= InjuryHealCost ? (Action)(() => HealInjury(x.InjuryName)) : () => { }));
        if (noInjuriesPrototype != null && _hero.Health.InjuryNames.None())
            Instantiate(noInjuriesPrototype, injuriesParent.transform);
    }
    
    private void HealHeroToFull()
    {
        party.HealHeroToFull(_hero.Character);
        party.UpdateClinicVouchersBy(-FullHealCost);
        Message.Publish(new UpdateClinic());
    }

    private void HealInjury(string injuryName)
    {
        _hero.HealInjuryByName(injuryName);
        party.UpdateClinicVouchersBy(-InjuryHealCost);
        Message.Publish(new HideTooltip());
        Message.Publish(new UpdateClinic());
    }
}
