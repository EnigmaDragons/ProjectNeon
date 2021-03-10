using TMPro;
using UnityEngine;

public sealed class LevelUpOptionsPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private GameObject optionParent;
    [SerializeField] private LevelUpOptionPresenter optionPrototype;
    [SerializeField] private GameObject[] toDestroyOnStart;

    private void Start() => toDestroyOnStart.ForEach(Destroy);

    public void Init(Hero hero)
        => Init(hero.Level, hero.Character.LevelUpTree.ForLevel(hero.Level));
    
    public void Init(int level, HeroLevelUpOption[] options)
    {
        if (levelLabel != null)
            levelLabel.text = level.ToString();
        options.ForEach(o => Instantiate(optionPrototype, optionParent.transform).Initialized(o));
    }
}
