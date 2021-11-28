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
        var r = Initialized(h.Stats, h.BaseStats, h.PrimaryStat);
        hpLabel.text = $"{h.CurrentHp}/{h.Stats.MaxHp()}";
        return r;
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

    public MemberStatPanel Initialized(IStats current, IStats baseStats, StatType primaryStat)
    {
        var stats = current;
        var difference = stats.Minus(baseStats);
        startingShieldLabel.text = $"{stats.StartingShield()}";
        startingShieldLabel.color = ColorFor(difference.StartingShield());
        maxShieldLabel.text = $"{stats.MaxShield()}";
        maxShieldLabel.color = ColorFor(difference.MaxShield());
        attackLabel.text = $"{stats.Attack()}";
        attackLabel.color = ColorFor(difference.Attack());
        attackLabel.gameObject.transform.parent.gameObject.SetActive(stats.Attack() > 0);
        attackLabel.font = primaryStat == StatType.Attack ? primaryStatFont : normalFont;
        attackNameLabel.font = primaryStat == StatType.Attack ? primaryStatFont : normalFont;
        magicLabel.text = $"{stats.Magic()}";
        magicLabel.color = ColorFor(difference.Magic());
        magicLabel.gameObject.transform.parent.gameObject.SetActive(stats.Magic() > 0);
        magicLabel.font = primaryStat == StatType.Magic ? primaryStatFont : normalFont;
        magicNameLabel.font = primaryStat == StatType.Magic ? primaryStatFont : normalFont;
        leaderLabel.text = $"{stats.Leadership()}";
        leaderLabel.color = ColorFor(difference.Leadership());
        leaderLabel.gameObject.transform.parent.gameObject.SetActive(stats.Leadership() > 0);
        leaderLabel.font = primaryStat == StatType.Leadership ? primaryStatFont : normalFont;
        leaderNameLabel.font = primaryStat == StatType.Leadership ? primaryStatFont : normalFont;
        armorLabel.text = $"{stats.Armor()}";
        armorLabel.color = ColorFor(difference.Armor());
        resistLabel.text = $"{stats.Resistance()}";
        resistLabel.color = ColorFor(difference.Resistance());
        econLabel.text = $"{stats.Economy()}";
        econLabel.color = ColorFor(difference.Economy());
        econLabel.gameObject.transform.parent.gameObject.SetActive(stats.Economy() > 0);
        econLabel.font = primaryStat == StatType.Economy ? primaryStatFont : normalFont;
        econNameLabel.font = primaryStat == StatType.Economy ? primaryStatFont : normalFont;
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
