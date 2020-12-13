using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroHpPresenter : OnMessage<PartyAdventureStateChanged, HeroStateChanged>
{
    [SerializeField] private Image bust;
    [SerializeField] private TextMeshProUGUI hpText;

    private Hero _hero;
    
    public void Init(Hero hero)
    {
        _hero = hero;
        bust.sprite = hero.Character.Bust;
        hpText.text = $"{hero.CurrentHp}/{hero.Stats.MaxHp()}";
    }

    protected override void Execute(PartyAdventureStateChanged msg) => Init(_hero);
    protected override void Execute(HeroStateChanged msg) => Init(_hero);
}
