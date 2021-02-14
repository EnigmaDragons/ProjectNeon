using TMPro;
using UnityEngine;

public sealed class MemberStatPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpLabel;
    [SerializeField] private TextMeshProUGUI maxShieldLabel;
    [SerializeField] private TextMeshProUGUI attackLabel;
    [SerializeField] private TextMeshProUGUI magicLabel;
    [SerializeField] private TextMeshProUGUI armorLabel;
    [SerializeField] private TextMeshProUGUI resistLabel;
    [SerializeField] private TextMeshProUGUI toughLabel;
    [SerializeField] private TextMeshProUGUI leaderLabel;

    public MemberStatPanel Initialized(Hero h)
    {
        var m = h.Stats;
        hpLabel.text = $"{h.CurrentHp}/{m.MaxHp()}";
        maxShieldLabel.text = $"{m.MaxShield()}";
        attackLabel.text = $"{m.Attack()}";
        magicLabel.text = $"{m.Magic()}";
        armorLabel.text = $"{m.Armor()}";
        resistLabel.text = $"{m.Resistance()}";
        toughLabel.text = $"{m.Toughness()}";
        leaderLabel.text = $"{m.Leadership()}";
        
        return this;
    }

    public MemberStatPanel Initialized(IStats s)
    {
        hpLabel.text = $"{s.MaxHp()}";
        maxShieldLabel.text = $"{s.MaxShield()}";
        attackLabel.text = $"{s.Attack()}";
        magicLabel.text = $"{s.Magic()}";
        armorLabel.text = $"{s.Armor()}";
        resistLabel.text = $"{s.Resistance()}";
        toughLabel.text = $"{s.Toughness()}";
        leaderLabel.text = $"{s.Leadership()}";
        return this;
    }
}
