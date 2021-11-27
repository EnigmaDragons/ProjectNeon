using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpSelectionPresenterV4 : OnMessage<LevelUpOptionSelected, HeroStateChanged>
{
    [SerializeField] private TextMeshProUGUI headerLabel;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Image bust;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private LevelUpOptionsPresenterV4 optionsPresenter;
    [SerializeField] private GameObject[] toEnableOnRender;

    private Hero _hero;
    
    public LevelUpSelectionPresenterV4 Initialized(Hero hero)
    {
        _hero = hero;
        Render();
        return this;
    }

    private void Render()
    {
        if (_hero == null)
            return;
        
        gameObject.SetActive(true);
        toEnableOnRender.ForEach(g => g.SetActive(false));
        bust.sprite = _hero.Character.Bust;
        stats.Initialized(_hero.Stats);
        levelLabel.text = $"Level {_hero.Level.ToString()}";
        optionsPresenter.Init(_hero);
        
        toEnableOnRender.ForEach(g => g.SetActive(true));
        headerLabel.transform.localScale = new Vector3(4, 4, 4);
        headerLabel.transform.DOScale(1, 0.6f);
        var levelOriginalScale = levelLabel.transform.localScale;
        levelLabel.transform.localScale = new Vector3(0, 0, 0);
        this.ExecuteAfterDelay(0.6f, () => levelLabel.transform.DOScale(levelOriginalScale.x, 0.6f));
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
