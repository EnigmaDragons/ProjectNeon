using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HeroLevelUpTreeUiController : OnMessage<ShowHeroLevelUpPathway, HideHeroLevelUpPathway>
{
    [SerializeField] private GameObject target;
    [SerializeField] private Image bust;
    [SerializeField] private Localize classLocalize;
    [SerializeField] private Localize nameLocalize;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private LevelUpPathwayPresenter presenter;

    private void Awake() => target.SetActive(false);
    
    protected override void Execute(ShowHeroLevelUpPathway msg)
    {
        presenter.Init(msg.Hero.LevelUpTree);
        bust.sprite = msg.Hero.Bust;
        classLocalize.SetTerm(msg.Hero.ClassTerm());
        nameLocalize.SetTerm(msg.Hero.NameTerm());
        stats.Initialized(msg.Hero.Stats);
        target.SetActive(true);
    }

    protected override void Execute(HideHeroLevelUpPathway msg)
    {
        target.SetActive(false);
    }
}
