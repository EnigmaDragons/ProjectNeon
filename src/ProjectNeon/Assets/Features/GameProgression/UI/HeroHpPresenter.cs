using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroHpPresenter : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private Image bust;
    [SerializeField] private TextMeshProUGUI hpText;

    private Hero _hero;
    
    public void Init(Hero hero, int hp)
    {
        _hero = hero;
        bust.sprite = hero.Bust;
        hpText.text = $"{hp}/{hero.Stats.MaxHP()}";
    }

    protected override void Execute(PartyAdventureStateChanged msg)
    {
        Init(_hero, msg.State.CurrentHpOf(_hero));
    }
}
