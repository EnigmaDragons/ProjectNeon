using TMPro;
using UnityEngine;

public sealed class MemberStatPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpLabel;
    [SerializeField] private TextMeshProUGUI startingShieldLabel;
    [SerializeField] private TextMeshProUGUI maxShieldLabel;
    [SerializeField] private TextMeshProUGUI attackLabel;
    [SerializeField] private TextMeshProUGUI magicLabel;
    [SerializeField] private TextMeshProUGUI leaderLabel;
    [SerializeField] private TextMeshProUGUI armorLabel;
    [SerializeField] private TextMeshProUGUI resistLabel;
    [SerializeField] private TextMeshProUGUI toughLabel;

    public MemberStatPanel Initialized(Hero h)
    {
        var m = h.Stats;
        hpLabel.text = $"{h.CurrentHp}/{m.MaxHp()}";
        startingShieldLabel.text = $"{m.StartingShield()}";
        maxShieldLabel.text = $"{m.MaxShield()}";
        attackLabel.text = $"{m.Attack()}";
        magicLabel.text = $"{m.Magic()}";
        leaderLabel.text = $"{m.Leadership()}";
        armorLabel.text = $"{m.Armor()}";
        resistLabel.text = $"{m.Resistance()}";
        toughLabel.text = $"{m.Toughness()}";
        return this;
    }

    public MemberStatPanel Initialized(IStats s)
    {
        hpLabel.text = $"{s.MaxHp()}";
        startingShieldLabel.text = $"{s.StartingShield()}";
        maxShieldLabel.text = $"{s.MaxShield()}";
        attackLabel.text = $"{s.Attack()}";
        magicLabel.text = $"{s.Magic()}";
        leaderLabel.text = $"{s.Leadership()}";
        armorLabel.text = $"{s.Armor()}";
        resistLabel.text = $"{s.Resistance()}";
        toughLabel.text = $"{s.Toughness()}";
        return this;
    }
}
