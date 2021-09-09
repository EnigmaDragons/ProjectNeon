using TMPro;
using UnityEngine;

public sealed class MemberStatPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpLabel;
    [SerializeField] private TextMeshProUGUI startingShieldLabel;
    [SerializeField] private TextMeshProUGUI maxShieldLabel;
    [SerializeField] private TextMeshProUGUI attackLabel;
    [SerializeField] private TextMeshProUGUI attackNameLabel;
    [SerializeField] private TextMeshProUGUI magicLabel;
    [SerializeField] private TextMeshProUGUI magicNameLabel;
    [SerializeField] private TextMeshProUGUI leaderLabel;
    [SerializeField] private TextMeshProUGUI leaderNameLabel;
    [SerializeField] private TextMeshProUGUI armorLabel;
    [SerializeField] private TextMeshProUGUI resistLabel;
    [SerializeField] private TextMeshProUGUI econLabel;
    [SerializeField] private TextMeshProUGUI econNameLabel;
    [SerializeField] private TMP_FontAsset normalFont;
    [SerializeField] private TMP_FontAsset primaryStatFont;
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
        attackLabel.gameObject.transform.parent.gameObject.SetActive(m.Attack() > 0);
        attackLabel.font = h.PrimaryStat == StatType.Attack ? primaryStatFont : normalFont;
        attackNameLabel.font = h.PrimaryStat == StatType.Attack ? primaryStatFont : normalFont;
        magicLabel.text = $"{m.Magic()}";
        magicLabel.color = ColorFor(difference.Magic());
        magicLabel.gameObject.transform.parent.gameObject.SetActive(m.Magic() > 0);
        magicLabel.font = h.PrimaryStat == StatType.Magic ? primaryStatFont : normalFont;
        magicNameLabel.font = h.PrimaryStat == StatType.Magic ? primaryStatFont : normalFont;
        leaderLabel.text = $"{m.Leadership()}";
        leaderLabel.color = ColorFor(difference.Leadership());
        leaderLabel.gameObject.transform.parent.gameObject.SetActive(m.Leadership() > 0);
        leaderLabel.font = h.PrimaryStat == StatType.Leadership ? primaryStatFont : normalFont;
        leaderNameLabel.font = h.PrimaryStat == StatType.Leadership ? primaryStatFont : normalFont;
        armorLabel.text = $"{m.Armor()}";
        armorLabel.color = ColorFor(difference.Armor());
        resistLabel.text = $"{m.Resistance()}";
        resistLabel.color = ColorFor(difference.Resistance());
        econLabel.text = $"{m.Economy()}";
        econLabel.color = ColorFor(difference.Economy());
        econLabel.gameObject.transform.parent.gameObject.SetActive(m.Economy() > 0);
        econLabel.font = h.PrimaryStat == StatType.Economy ? primaryStatFont : normalFont;
        econNameLabel.font = h.PrimaryStat == StatType.Economy ? primaryStatFont : normalFont;
        return this;
    }

    public MemberStatPanel Initialized(IStats s)
    {
        hpLabel.text = $"{s.MaxHp()}";
        startingShieldLabel.text = $"{s.StartingShield()}";
        maxShieldLabel.text = $"{s.MaxShield()}";
        attackLabel.text = $"{s.Attack()}";
        attackLabel.gameObject.transform.parent.gameObject.SetActive(s.Attack() > 0);
        magicLabel.text = $"{s.Magic()}";
        magicLabel.gameObject.transform.parent.gameObject.SetActive(s.Magic() > 0);
        leaderLabel.text = $"{s.Leadership()}";
        leaderLabel.gameObject.transform.parent.gameObject.SetActive(s.Leadership() > 0);
        armorLabel.text = $"{s.Armor()}";
        resistLabel.text = $"{s.Resistance()}";
        econLabel.text = $"{s.Economy()}";
        econLabel.gameObject.transform.parent.gameObject.SetActive(s.Economy() > 0);
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
