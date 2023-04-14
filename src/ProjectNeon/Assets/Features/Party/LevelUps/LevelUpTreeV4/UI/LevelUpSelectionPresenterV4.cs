using System.Linq;
using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpSelectionPresenterV4 : OnMessage<LevelUpOptionSelected>, ILocalizeTerms
{
    [SerializeField] private CurrentAdventure adventure;
    
    [SerializeField] private TextMeshProUGUI headerLabel;
    [SerializeField] private Localize levelLabel;
    [SerializeField] private GameObject heroNameObject;
    [SerializeField] private Localize heroNameLocalize;
    [SerializeField] private GameObject heroClassObject;
    [SerializeField] private Localize heroClassLocalize;
    [SerializeField] private Image bust;
    [SerializeField] private ResourceCounterPresenter primaryResourceCounter;
    [SerializeField] private ResourceCounterPresenter secondaryResourceCounter;
    [SerializeField] private DeterminedNodeInfo nodeInfo;
    
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI faintLevelLabel;
    [SerializeField] private Image faintBust;
    [SerializeField] private Localize faintClassLocalize;
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

        var member = _hero.AsMember(-1);
        gameObject.SetActive(true);
        toEnableOnRender.ForEach(g => g.SetActive(false));
        toEnableOnFinished.ForEach(g => g.SetActive(false));
        toDisableOnFinished.ForEach(g => g.SetActive(true));
        arrowsAnim.color = Color.white.Transparent();
        fader.color = faderStartColor;
        fader.DOColor(faderStartColor.Transparent(), 1).SetEase(Ease.InOutQuad);
        bust.sprite = _hero.Character.Bust;
        primaryResourceCounter.Init(member, _hero.PrimaryResource);
        if (secondaryResourceCounter != null)
        { 
            if(member.State.ResourceTypes.Length < 2)
                secondaryResourceCounter.Hide();
            else
                secondaryResourceCounter.Init(member, member.State.ResourceTypes[1]);
        }

        faintBust.sprite = _hero.Character.Bust;
        faintBust.color = new Color(1, 1, 1, 1 / 255f);
        faintLevelLabel.text = _hero.Level.ToString(); 
        faintLevelLabel.color = new Color(1, 1, 1, 1 / 255f);
        faintClassLocalize.SetTerm(_hero.ClassTerm);
        faintClassName.color = new Color(1, 1, 1, 1 / 255f);
        stats.Initialized(_hero);
        heroNameLocalize.SetTerm(_hero.NameTerm);
        heroClassLocalize.SetTerm(_hero.ClassTerm);
        levelLabel.SetFinalText($"{"LevelUps/Level".ToLocalized()} {_hero.Level.ToString()}");
        var adventureMode = adventure != null ? adventure.Adventure.Mode : AdventureMode.Standard;
        optionsPresenter.Init(adventureMode, _hero);
        
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
        AllMetrics.PublishLevelUpOptionSelection(_hero.NameTerm.ToEnglish(), _hero.Level, msg.Selected.Description, msg.Options.Select(o => o.Description).ToArray());
        nodeInfo.HeroLevelUpAugments = Maybe<StaticEquipment[]>.Missing();
        nodeInfo.DraftLevelUpOptions = Maybe<DraftWildLevelUpData[]>.Missing();
        msg.Selected.SelectAsLevelUp(_hero);
        Message.Publish(new LevelUpClicked(transform));
        
        this.ExecuteAfterDelay(() =>
        {
            gameObject.SetActive(false);
            Message.Publish(new HeroLevelledUp());
        }, finishAnimationDuration);
    }

    public string[] GetLocalizeTerms()
        => new [] {"LevelUps/Level"};
}
