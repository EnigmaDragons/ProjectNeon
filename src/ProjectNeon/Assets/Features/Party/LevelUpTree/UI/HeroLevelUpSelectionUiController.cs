using UnityEngine;

public class HeroLevelUpSelectionUiController : OnMessage<LevelUpHero>
{
    [SerializeField] private GameObject target;
    [SerializeField] private HeroLevelUpSelectionPresenter presenter;

    private void Start() => target.SetActive(false);

    protected override void Execute(LevelUpHero msg)
    {
        presenter.Initialized(msg.Hero);
        target.SetActive(true);
    }
}
