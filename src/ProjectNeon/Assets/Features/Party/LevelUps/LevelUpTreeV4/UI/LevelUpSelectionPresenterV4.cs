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
    [SerializeField] private ResourceCounterPresenter primaryResourceCounter;
    
    [SerializeField] private TextMeshProUGUI faintLevelLabel;
    [SerializeField] private Image faintBust;
    [SerializeField] private TextMeshProUGUI faintClassName;
    
    [SerializeField] private MemberStatDiffPanel stats;
    
    [SerializeField] private Image fader;
    [SerializeField] private Color faderStartColor;
    [SerializeField] private RawImage arrowsAnim;
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
        toDisableOnFinished.ForEach(g => g.SetActive(true));
        arrowsAnim.color = Color.white.Transparent();
        fader.color = faderStartColor;
        fader.DOColor(faderStartColor.Transparent(), 1).SetEase(Ease.InOutQuad);
        bust.sprite = _hero.Character.Bust;
        primaryResourceCounter.Init(_hero.AsMember(-1), _hero.PrimaryResource);
        faintBust.sprite = _hero.Character.Bust;
        faintBust.color = new Color(1, 1, 1, 1 / 255f);
        faintLevelLabel.text = _hero.Level.ToString(); 
        faintLevelLabel.color = new Color(1, 1, 1, 1 / 255f);
        faintClassName.text = _hero.Class;
        faintClassName.color = new Color(1, 1, 1, 1 / 255f);
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
        levelLabel.transform.DOScale(levelOriginalScale.x, 0.6f).SetDelay(0.6f);
    }

    protected override void Execute(LevelUpOptionSelected msg)
    {        
        if (_hero == null)
            return;
        
        Log.Info("Level Up Selection Presenter V4 Handling Selection Option");
        toEnableOnFinished.ForEach(g => g.SetActive(true));
        toDisableOnFinished.ForEach(g => g.SetActive(false));
        arrowsAnim.DOColor(Color.white.WithAlpha(0.4f), 2).SetEase(Ease.InOutQuad);
        fader.color = faderStartColor.Transparent();
        fader.DOColor(faderStartColor, 1).SetEase(Ease.InOutQuad).SetDelay(2);
        faintBust.DOColor(Color.white, 2.4f).SetEase(Ease.InQuad);
        faintLevelLabel.DOColor(Color.white, 2.4f).SetEase(Ease.InQuad);
        faintClassName.DOColor(Color.white, 2.4f).SetEase(Ease.Linear);
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
