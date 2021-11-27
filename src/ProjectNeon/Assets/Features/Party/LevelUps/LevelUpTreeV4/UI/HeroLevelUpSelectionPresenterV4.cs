using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroLevelUpSelectionPresenterV4 : OnMessage<LevelUpOptionSelected, HeroStateChanged>
{
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Image bust;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private LevelUpOptionsPresenter optionsPresenter;

    private Hero _hero;
    
    public HeroLevelUpSelectionPresenterV4 Initialized(Hero hero)
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
        optionsPresenter.Init(_hero, useV4: true);
    }

    protected override void Execute(LevelUpOptionSelected msg)
    {        
        if (_hero == null)
            return;
        
        AllMetrics.PublishLevelUpOptionSelection(_hero.Name, _hero.Level, msg.Selected.Description, msg.Options.Select(o => o.Description).ToArray());
        msg.Selected.SelectAsLevelUp(_hero);
        gameObject.SetActive(false);
        Message.Publish(new HeroLevelledUp());
    }

    protected override void Execute(HeroStateChanged msg)
    {                
        if (_hero == null)
            return;
        
        Initialized(_hero);
    }
}
