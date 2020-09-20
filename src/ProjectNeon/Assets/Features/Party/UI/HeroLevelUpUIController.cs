
using UnityEngine;

public class HeroLevelUpUIController : OnMessage<LevelUpHero, Finished<LevelUpHero>>
{
    [SerializeField] private GameObject view;
    [SerializeField] private HeroDetailsPanel details;
    [SerializeField] private HeroFlexibleLevelUpPresenter levelUpPresenter;
    
    protected override void Execute(LevelUpHero msg)
    {
        details.Initialized(msg.Hero, msg.Hero.AsMember(0));
        levelUpPresenter.Initialize(msg.Hero);
        view.SetActive(true);
    }

    protected override void Execute(Finished<LevelUpHero> msg)
    {
        view.SetActive(false);
    }
}
