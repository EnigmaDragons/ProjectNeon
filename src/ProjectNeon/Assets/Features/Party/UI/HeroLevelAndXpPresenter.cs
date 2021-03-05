using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroLevelAndXpPresenter : MonoBehaviour
{
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private XpPresenter xp;
    
    private Hero _hero;

    public HeroLevelAndXpPresenter Initialized(Hero h)
    {        
        _hero = h;
        heroBust.sprite = _hero.Character.Bust;
        levelLabel.text = _hero.Level.ToString();
        xp.Init(_hero);
        return this;
    }

    public void ShowPreview(HeroLevels levels)
    {
        if (levels.CurrentLevel > _hero.Level)
        {
            levelLabel.text = levels.CurrentLevel.ToString();
            levelLabel.transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 1);
        }

        xp.SmoothTransitionTo(levels.NextLevelProgress);
    }
}
