using UnityEngine;

public sealed class HeroLevelUpUiController : OnMessage<ShowHeroLevelUpPathway, HideHeroLevelUpPathway>
{
    [SerializeField] private GameObject target;
    [SerializeField] private LevelUpPathwayPresenter presenter;

    private void Awake() => target.SetActive(false);
    
    protected override void Execute(ShowHeroLevelUpPathway msg)
    {
        presenter.Init(msg.Hero.LevelUpTree);
        target.SetActive(true);
    }

    protected override void Execute(HideHeroLevelUpPathway msg)
    {
        target.SetActive(false);
    }
}
