using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class LevelUpOptionsPresenterV4 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TextMeshProUGUI promptLabel;
    [SerializeField] private GameObject optionParent;
    [SerializeField] private LevelUpOptionPresenterV4 basicOptionPrototype;
    [SerializeField] private CustomLevelUpPresenters presenters;
    [SerializeField] private GameObject[] toDestroyOnStart;
    [SerializeField] private float unfoldDuration = 2f;
    [SerializeField] private float unfoldInitialDelay = 3f;
    [SerializeField] private float unfoldGapBetweenItems = 0.6f;

    private readonly List<GameObject> _options = new List<GameObject>();
    
    private void Start() => toDestroyOnStart.ForEach(Destroy);

    public void Init(Hero hero)
    {
        var reward = hero.NextLevelUpRewardV4;
        Init(hero, hero.Levels.NextLevelUpLevel, reward.OptionsPrompt, reward.GenerateOptions(hero));
    }

    public void Init(Hero hero, int level, string optionPrompt, LevelUpOption[] options)
    {
        _options.ForEach(Destroy);
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
                this.ExecuteAfterDelay(() => Message.Publish(new PlayUiSound("LevelUpOptionReveal", presenter.transform)), unfoldDelay);
                unfoldDelay += unfoldGapBetweenItems;
                _options.Add(presenter);
            });

        this.ExecuteAfterDelay(() =>
        {
            promptLabel.text = optionPrompt;
            promptLabel.transform.localScale = new Vector3(4, 4, 4);
            promptLabel.transform.DOScale(1, 0.6f);
        }, unfoldDelay);
    }
}
