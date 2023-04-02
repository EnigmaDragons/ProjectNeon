using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HealHeroButton : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Button button;
    [SerializeField] private Localize costLabel;
    [SerializeField] private int cost;
    [SerializeField] private int reviveCost;

    private int _healCost;
    private BaseHero _hero;
    
    private void Awake() => button.onClick.AddListener(HealToFullIfCanAfford);

    public void Init(BaseHero h)
    {
        _hero = h;
        _healCost = party.CurrentHpOf(_hero) > 0 ? cost : reviveCost;
        gameObject.SetActive(party.CurrentHpOf(_hero) < _hero.Stats.MaxHp());
        costLabel.SetFinalText($"{"Clinics/Heal".ToLocalized()} - {_healCost}");
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

    public string[] GetLocalizeTerms()
        => new [] {"Clinics/Heal"};
}
