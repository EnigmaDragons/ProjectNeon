using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class LevelUpOptionsPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private GameObject optionParent;
    [SerializeField] private LevelUpOptionPresenter optionPrototype;
    [SerializeField] private GameObject[] toDestroyOnStart;

    private readonly List<GameObject> _options = new List<GameObject>();
    
    private void Start() => toDestroyOnStart.ForEach(Destroy);

    public void Init(Hero hero)
        => Init(hero.Level + 1 - hero.Levels.UnspentLevelUpPoints, hero.Character.LevelUpTree.ForLevel(hero.Level + 1 - hero.Levels.UnspentLevelUpPoints));
    
    public void Init(int level, HeroLevelUpOption[] options)
    {
        _options.ForEach(Destroy);
        _options.Clear();
        if (levelLabel != null)
            levelLabel.text = level.ToString();
        options.ForEach(o => _options.Add(Instantiate(optionPrototype, optionParent.transform).Initialized(o).gameObject));
    }
}
