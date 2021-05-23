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
    [SerializeField] private TextMeshProUGUI econLabel;
    [SerializeField] private Color buffColor = Color.green;
    [SerializeField] private Color debuffColor = Color.red;

    public MemberStatPanel Initialized(Hero h)
    {
        var m = h.Stats;
        var difference = m.Minus(h.BaseStats);
        hpLabel.text = $"{h.CurrentHp}/{m.MaxHp()}";
        startingShieldLabel.text = $"{m.StartingShield()}";
        startingShieldLabel.color = ColorFor(difference.StartingShield());
        maxShieldLabel.text = $"{m.MaxShield()}";
        maxShieldLabel.color = ColorFor(difference.MaxShield());
        attackLabel.text = $"{m.Attack()}";
        attackLabel.color = ColorFor(difference.Attack());
        magicLabel.text = $"{m.Magic()}";
        magicLabel.color = ColorFor(difference.Magic());
        leaderLabel.text = $"{m.Leadership()}";
        leaderLabel.color = ColorFor(difference.Leadership());
        armorLabel.text = $"{m.Armor()}";
        armorLabel.color = ColorFor(difference.Armor());
        resistLabel.text = $"{m.Resistance()}";
        resistLabel.color = ColorFor(difference.Resistance());
        toughLabel.text = $"{m.Toughness()}";
        toughLabel.color = ColorFor(difference.Toughness());
        econLabel.text = $"{m.Economy()}";
        econLabel.color = ColorFor(difference.Economy());
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
        econLabel.text = $"{s.Economy()}";
        return this;
    }

    private Color ColorFor(float delta)
    {
        if (delta < 0)
            return debuffColor;
        if (delta > 0)
            return buffColor;
        return Color.white;
    }
}
