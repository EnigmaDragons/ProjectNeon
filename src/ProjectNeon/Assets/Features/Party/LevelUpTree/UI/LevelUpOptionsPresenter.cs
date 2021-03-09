using TMPro;
using UnityEngine;

public sealed class LevelUpOptionsPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private GameObject optionParent;
    [SerializeField] private LevelUpOptionPresenter optionPrototype;

    public void Init(int level, HeroLevelUpOption[] options)
    {
        levelLabel.text = level.ToString();
        options.ForEach(o => Instantiate(optionPrototype, optionParent.transform).Initialized(o));
    }
}
