using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HeroLevelUpTreeUiController : OnMessage<ShowHeroLevelUpPathway, HideHeroLevelUpPathway>
{
    [SerializeField] private GameObject target;
    [SerializeField] private Image bust;
    [SerializeField] private TextMeshProUGUI classLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private LevelUpPathwayPresenter presenter;

    private void Awake() => target.SetActive(false);
    
    protected override void Execute(ShowHeroLevelUpPathway msg)
    {
        presenter.Init(msg.Hero.LevelUpTree);
        bust.sprite = msg.Hero.Bust;
        classLabel.text = msg.Hero.Class;
        nameLabel.text = msg.Hero.Name;
        stats.Initialized(msg.Hero.Stats);
        target.SetActive(true);
    }

    protected override void Execute(HideHeroLevelUpPathway msg)
    {
        target.SetActive(false);
    }
}
