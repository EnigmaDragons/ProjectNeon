using UnityEngine;

public sealed class AdventureHeroUiSummary : OnMessage<PartyStateChanged>
{
    [SerializeField] private HeroHpPresenter hpUi;
    [SerializeField] private HealHeroButton healButton;

    private Hero _hero;
    
    public void Init(Hero hero)
    {
        _hero = hero;
        UpdateUi();
    }

    protected override void Execute(PartyStateChanged msg) => UpdateUi();

    private void UpdateUi()
    {
        hpUi.Init(_hero);
        healButton.Init(_hero.Character);
    }
}
