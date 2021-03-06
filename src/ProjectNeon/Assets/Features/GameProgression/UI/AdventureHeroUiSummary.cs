using System.Linq;
using TMPro;
using UnityEngine;

public sealed class AdventureHeroUiSummary : OnMessage<PartyStateChanged>
{
    [SerializeField] private HeroHpPresenter hpUi;
    [SerializeField] private HealHeroButton healButton;
    [SerializeField] private GameObject injuryPanel;
    [SerializeField] private TextMeshProUGUI injuryCounter;
    [SerializeField] private XpAndLevelUpPresenter xpAndLevels;

    private Hero _hero;
    private bool _canHealAnywhere;
    
    public void Init(Hero hero, bool canHealAnywhere)
    {
        _hero = hero;
        _canHealAnywhere = canHealAnywhere;
        UpdateUi();
    }

    protected override void Execute(PartyStateChanged msg) => UpdateUi();

    private void UpdateUi()
    {
        hpUi.Init(_hero);
        healButton.Init(_hero.Character);
        xpAndLevels.Init(_hero);
        healButton.gameObject.SetActive(_canHealAnywhere);
        
        var injuries = _hero.Health.InjuryCounts;
        var numInjuries = injuries.Sum(x => x.Value);
        injuryPanel.SetActive(!_canHealAnywhere && injuries.Any());
        injuryCounter.text = numInjuries > 1 ? numInjuries.ToString() : "";
    }
}
