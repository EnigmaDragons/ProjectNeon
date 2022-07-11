using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class LevelUpOptionsPresenterV4 : MonoBehaviour
{
    [SerializeField] private LevelUpOptions draftOptions;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TextMeshProUGUI promptLabel;
    [SerializeField] private GameObject optionParent;
    [SerializeField] private LevelUpOptionPresenterV4 basicOptionPrototype;
    [SerializeField] private CustomLevelUpPresenters presenters;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject[] toDestroyOnStart;
    [SerializeField] private float unfoldDuration = 2f;
    [SerializeField] private float unfoldInitialDelay = 3f;
    [SerializeField] private float unfoldGapBetweenItems = 0.6f;

    private bool _initialized;
    private readonly Dictionary<LevelUpOption, GameObject> _options = new Dictionary<LevelUpOption, GameObject>();

    private void Start() => InitComponent();

    private void InitComponent()
    {
        if (_initialized)
            return;
        
        _initialized = true;
        toDestroyOnStart?.ToList().ForEach(DestroyImmediate);
    }

    public void ClearUnselectedOptions(LevelUpOption selected)
    {
        _options.ForEach(o =>
        {
            if (o.Value != null && o.Key != selected)
                o.Value.SetActive(false);
        });
        promptLabel.text = "";
    }
    
    public void Init(AdventureMode mode, Hero hero)
    {
        if (mode == AdventureMode.Draft && hero.Levels.NextLevelUpLevel != 4) // Only use draft option for non-Basic Card picks
            Init(hero, hero.Levels.NextLevelUpLevel, "Select An Option!", draftOptions.Generate(hero));
        else
        {
            var reward = hero.NextLevelUpRewardV4;
            Init(hero, hero.Levels.NextLevelUpLevel, reward.OptionsPrompt, reward.GenerateOptions(hero, party));
        }
    }

    public void Init(Hero hero, int level, string optionPrompt, LevelUpOption[] options)
    {
        InitComponent();
        _options.ForEach(o => DestroyImmediate(o.Value));
        _options.Clear();
        if (levelLabel != null)
            levelLabel.text = level.ToString();
        promptLabel.text = "";

        var unfoldDelay = unfoldInitialDelay;
        options.Where(x => x.IsFunctional)
            .ForEach(o =>
            {
                var currentDelay = unfoldDelay;
                var presenter = o.UseCustomOptionPresenter
                    ? o.CreatePresenter(new LevelUpCustomPresenterContext(optionParent.transform, presenters, hero, o, options))
                    : Instantiate(basicOptionPrototype, optionParent.transform).Initialized(o, options).gameObject;
                presenter.transform.localRotation = Quaternion.Euler(0, 90, 0);
                presenter.transform.DORotate(Vector3.zero, unfoldDuration).SetEase(Ease.OutQuint).SetDelay(currentDelay);
                this.ExecuteAfterDelay(() =>
                {
                    if (presenter != null)
                        Message.Publish(new PlayUiSound("LevelUpOptionReveal", presenter.transform));
                }, unfoldDelay);
                unfoldDelay += unfoldGapBetweenItems;
                _options[o] = presenter;
            });

        this.ExecuteAfterDelay(() =>
        {
            promptLabel.text = optionPrompt;
            promptLabel.transform.localScale = new Vector3(4, 4, 4);
            promptLabel.transform.DOScale(1, 0.6f);
        }, unfoldDelay);
    }
}
