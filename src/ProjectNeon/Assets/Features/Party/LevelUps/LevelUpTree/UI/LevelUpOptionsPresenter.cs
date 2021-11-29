using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public sealed class LevelUpOptionsPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private GameObject optionParent;
    [SerializeField] private LevelUpOptionPresenter optionPrototype;
    [SerializeField] private GameObject[] toDestroyOnStart;
    [SerializeField] private PartyAdventureState party;

    private readonly List<GameObject> _options = new List<GameObject>();
    
    private void Start() => toDestroyOnStart.ForEach(Destroy);

    public void Init(Hero hero, bool useV4 = false)
        => Init(hero.Levels.NextLevelUpLevel, 
            useV4 
                ? hero.Character.LevelUpTreeV4.ForLevel(hero.Levels.NextLevelUpLevel).GenerateOptions(hero, party)
                : hero.Character.LevelUpTree.ForLevel(hero.Levels.NextLevelUpLevel));
    
    public void Init(int level, LevelUpOption[] options)
    {
        _options.ForEach(Destroy);
        _options.Clear();
        if (levelLabel != null)
            levelLabel.text = level.ToString();
        options.Where(x => x.IsFunctional)
            .ForEach(o => _options.Add(Instantiate(optionPrototype, optionParent.transform).Initialized(o, options).gameObject));
    }
}
