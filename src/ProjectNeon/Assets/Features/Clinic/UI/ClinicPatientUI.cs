using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicPatientUI : OnMessage<UpdateClinic, HeroStateChanged, PartyAdventureStateChanged>
{
    [SerializeField] private Localize nameLocalize;
    [SerializeField] private HeroHpPresenter hpPresenter;
    [SerializeField] private Button healToFullButton;
    [SerializeField] private Button viewHeroDetailButton;
    [SerializeField] private TextMeshProUGUI healToFullCostLabel;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject injuriesParent;
    [SerializeField] private HealInjuryButton healInjuryButtonPrototype;
    
    private Hero _hero;
    private ClinicCostCalculator _cost;
    private int _fullHealCost;
    private int _injuryHealCost;
    
    private void Awake()
    {
        healToFullButton.onClick.AddListener(HealHeroToFull);
        viewHeroDetailButton.onClick.AddListener(ViewHeroDetail);
        injuriesParent.DestroyAllChildren();
    }

    private void ViewHeroDetail() => Message.Publish(new ShowHeroDetailsView(_hero, Maybe<Member>.Missing()));
    
    public ClinicPatientUI Initialized(Hero h, ClinicCostCalculator cost)
    {
        _hero = h;
        _cost = cost;
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
        _fullHealCost = _cost.GetFullHealCost(_hero);
        _injuryHealCost = _cost.GetInjuryHealCost();
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
        healToFullButton.gameObject.SetActive(party.Credits >= _fullHealCost && _hero.CurrentHp < _hero.Stats.MaxHp());
        injuriesParent.DestroyAllChildren();
        _hero.Health.AllInjuries.ForEach(x => Instantiate(healInjuryButtonPrototype, injuriesParent.transform)
            .Init(x, _injuryHealCost, party.Credits >= _injuryHealCost ? (Action)(() => HealInjury(x.InjuryName)) : () => { }));
    }
    
    private void HealHeroToFull()
    {
        party.HealHeroToFull(_hero.Character);
        party.UpdateCreditsBy(-_fullHealCost);
        _cost.RequestClinicHealService();
        Message.Publish(new UpdateClinic());
    }

    private void HealInjury(string injuryName)
    {
        _hero.HealInjuryByName(injuryName);
        party.UpdateCreditsBy(-_injuryHealCost);
        _cost.RequestClinicHealService();
        Message.Publish(new UpdateClinic());
    }
}
