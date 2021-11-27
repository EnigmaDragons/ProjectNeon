using System.Collections.Generic;
using System.Linq;
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
        if (promptLabel != null)
            promptLabel.text = optionPrompt;
        options.Where(x => x.IsFunctional)
            .ForEach(o => _options.Add(o.UseCustomOptionPresenter
                ? o.CreatePresenter(new LevelUpCustomPresenterContext(optionParent.transform, presenters, hero, o, options))
                : Instantiate(basicOptionPrototype, optionParent.transform).Initialized(o, options).gameObject));
    }
}
