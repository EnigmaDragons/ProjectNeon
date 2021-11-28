using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpSelectionPresenterV4 : OnMessage<LevelUpOptionSelected>
{
    [SerializeField] private TextMeshProUGUI headerLabel;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private GameObject heroNameObject;
    [SerializeField] private TextMeshProUGUI heroNameLabel;
    [SerializeField] private GameObject heroClassObject;
    [SerializeField] private TextMeshProUGUI heroClassLabel;
    [SerializeField] private Image bust;
    [SerializeField] private TextMeshProUGUI faintLevelLabel;
    [SerializeField] private Image faintBust;
    [SerializeField] private MemberStatDiffPanel stats;
    [SerializeField] private LevelUpOptionsPresenterV4 optionsPresenter;
    [SerializeField] private GameObject[] toEnableOnRender;
    [SerializeField] private GameObject[] toEnableOnFinished;
    [SerializeField] private GameObject[] toDisableOnFinished;
    [SerializeField] private float finishAnimationDuration = 3f;

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
        toEnableOnFinished.ForEach(g => g.SetActive(false));
        bust.sprite = _hero.Character.Bust;
        faintBust.sprite = _hero.Character.Bust;
        faintLevelLabel.text = _hero.Level.ToString(); 
        stats.Initialized(_hero);
        heroNameLabel.text = _hero.DisplayName;
        heroClassLabel.text = _hero.Class;
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
        
        Log.Info("Level Up Selection Presenter V4 Handling Selection Option");
        toEnableOnFinished.ForEach(g => g.SetActive(true));
        toDisableOnFinished.ForEach(g => g.SetActive(false));
        faintBust.DOColor(Color.white, 2.4f).SetEase(Ease.InQuad);
        faintLevelLabel.DOColor(Color.white, 2.4f).SetEase(Ease.InQuad);
        optionsPresenter.ClearUnselectedOptions(msg.Selected);
        AllMetrics.PublishLevelUpOptionSelection(_hero.Name, _hero.Level, msg.Selected.Description, msg.Options.Select(o => o.Description).ToArray());
        msg.Selected.SelectAsLevelUp(_hero);
        Message.Publish(new LevelUpClicked(transform));
        
        this.ExecuteAfterDelay(() =>
        {
            gameObject.SetActive(false);
            Message.Publish(new HeroLevelledUp());
        }, finishAnimationDuration);
    }
}
