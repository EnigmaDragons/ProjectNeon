using UnityEngine;

public class HeroLevelUpSelectionUiController : OnMessage<LevelUpHero>
{
    [SerializeField] private GameObject target;
    [SerializeField] private HeroLevelUpSelectionPresenter presenter;

    private void Start() => target.SetActive(false);

    protected override void Execute(LevelUpHero msg)
    {
        Debug.Log($"Level Up {msg.Hero.Name}");
        presenter.Initialized(msg.Hero);
        target.SetActive(true);
    }
}
