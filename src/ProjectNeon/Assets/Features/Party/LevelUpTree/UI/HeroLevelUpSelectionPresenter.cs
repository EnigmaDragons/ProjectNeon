using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroLevelUpSelectionPresenter : OnMessage<LevelUpOptionSelected, HeroStateChanged>
{
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Image bust;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private LevelUpOptionsPresenter optionsPresenter;

    private Hero _hero;
    
    public HeroLevelUpSelectionPresenter Initialized(Hero hero)
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
        msg.Selected.Apply(_hero);
        gameObject.SetActive(false);
    }

    protected override void Execute(HeroStateChanged msg) => Initialized(_hero);
}
