using UnityEngine;
using UnityEngine.UI;

public class XpPresenter : OnMessage<HeroStateChanged>
{
    [SerializeField] private Image barFill;

    private Hero _hero;

    public void Init(Hero h)
    {
        _hero = h;
        barFill.fillAmount = (float)_hero.Levels.XpTowardsNextLevelUp / _hero.Levels.XpRequiredForNextLevel;
    }

    protected override void Execute(HeroStateChanged msg) => Init(_hero);
}
