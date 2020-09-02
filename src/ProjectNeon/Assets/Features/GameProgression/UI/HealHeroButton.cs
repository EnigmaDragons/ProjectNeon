using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HealHeroButton : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private int cost;
    [SerializeField] private int reviveCost;

    private int _healCost;
    private HeroCharacter _hero;
    
    private void Awake() => button.onClick.AddListener(HealToFullIfCanAfford);

    public void Init(HeroCharacter h)
    {
        _hero = h;
        _healCost = party.CurrentHpOf(_hero) > 0 ? cost : reviveCost;
        gameObject.SetActive(party.CurrentHpOf(_hero) < _hero.Stats.MaxHp());
        costLabel.text = $"Heal - {_healCost}";
    }

    private void HealToFullIfCanAfford()
    {
        if (party.Credits >= _healCost)
        {
            party.UpdateCreditsBy(-_healCost);
            party.HealHeroToFull(_hero);
            gameObject.SetActive(false);
        }
    }

    protected override void Execute(PartyAdventureStateChanged msg)
    {
        Init(_hero);
    }
}
