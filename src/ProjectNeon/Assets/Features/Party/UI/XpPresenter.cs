using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class XpPresenter : OnMessage<HeroStateChanged>
{
    [SerializeField] private Image barFill;

    private Hero _hero;

    public void Init(Hero h)
    {
        _hero = h;
        barFill.fillAmount = _hero.Levels.NextLevelProgress;
    }

    public void SmoothTransitionTo(float amount)
    {
        barFill.DOFillAmount(amount, 2);
    }

    protected override void Execute(HeroStateChanged msg) => Init(_hero);
}
