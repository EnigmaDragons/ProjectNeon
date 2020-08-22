using UnityEngine;
using UnityEngine.UI;

public sealed class HealHeroButton : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Button button;
    [SerializeField] private int cost;

    private HeroCharacter _hero;
    
    private void Awake() => button.onClick.AddListener(HealToFullIfCanAfford);

    public void Init(HeroCharacter h)
    {
        _hero = h;
        gameObject.SetActive(party.CurrentHpOf(_hero) < _hero.Stats.MaxHp());
    }

    private void HealToFullIfCanAfford()
    {
        if (party.Credits >= cost)
        {
            party.UpdateCreditsBy(-cost);
            party.HealHeroToFull(_hero);
            gameObject.SetActive(false);
        }
    }

    protected override void Execute(PartyAdventureStateChanged msg)
    {
        Init(_hero);
    }
}
