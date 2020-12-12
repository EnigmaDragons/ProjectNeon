using UnityEngine;

public sealed class AdventureHeroUiSummary : OnMessage<PartyStateChanged>
{
    [SerializeField] private HeroHpPresenter hpUi;
    [SerializeField] private HealHeroButton healButton;

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
        if (!_canHealAnywhere)
            healButton.gameObject.SetActive(false);
    }
}
