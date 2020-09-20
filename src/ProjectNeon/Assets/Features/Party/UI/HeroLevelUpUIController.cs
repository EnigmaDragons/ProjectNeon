using UnityEngine;

public class HeroLevelUpUIController : OnMessage<PartyAdventureStateChanged, LevelUpHero, Finished<LevelUpHero>>
{
    [SerializeField] private GameObject view;
    [SerializeField] private HeroDetailsPanel details;
    [SerializeField] private HeroFlexibleLevelUpPresenter levelUpPresenter;

    private Hero _hero;

    protected override void Execute(PartyAdventureStateChanged msg)
    {
        if (_hero == null)
            return;
        
        details.Initialized(_hero, _hero.AsMember(0));
        levelUpPresenter.Initialize(_hero);
    }

    protected override void Execute(LevelUpHero msg)
    {
        _hero = msg.Hero;
        details.Initialized(_hero, _hero.AsMember(0));
        levelUpPresenter.Initialize(_hero);
        view.SetActive(true);
    }

    protected override void Execute(Finished<LevelUpHero> msg)
    {
        _hero = null;
        view.SetActive(false);
    }
}
