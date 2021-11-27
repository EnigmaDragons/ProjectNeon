using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroLevelUpSelectionPresenterV2 : OnMessage<LevelUpOptionSelected, HeroStateChanged>
{
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Image bust;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private LevelUpOptionsPresenter optionsPresenter;

    private Hero _hero;
    
    public HeroLevelUpSelectionPresenterV2 Initialized(Hero hero)
    {
        _hero = hero;
        Render();
        return this;
    }

    private void Render()
    {
        if (_hero == null)
            return;
        
        bust.sprite = _hero.Character.Bust;
        stats.Initialized(_hero.Stats);
        levelLabel.text = $"Level {_hero.Level.ToString()}";
        optionsPresenter.Init(_hero);
    }

    protected override void Execute(LevelUpOptionSelected msg)
    {
        AllMetrics.PublishLevelUpOptionSelection(_hero.Name, _hero.Level, msg.Selected.Description, msg.Options.Select(o => o.Description).ToArray());
        msg.Selected.SelectAsLevelUp(_hero);
        gameObject.SetActive(false);
        Message.Publish(new HeroLevelledUp());
    }

    protected override void Execute(HeroStateChanged msg) => Initialized(_hero);
}
