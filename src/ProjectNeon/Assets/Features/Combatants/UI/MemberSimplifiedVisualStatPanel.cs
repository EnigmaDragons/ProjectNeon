using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemberSimplifiedVisualStatPanel : MemberUiBase
{
    [SerializeField] private UIHPBarController hpBar;
    [SerializeField] private GameObject cardPlaysItem;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI cardPlaysLabel;
    [SerializeField] private GameObject resourceGainItem;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI resourceGainLabel;
    [SerializeField] private Image resourceGainIcon;
    [SerializeField] private GameObject leadershipItem;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI leadershipLabel;
    [SerializeField] private GameObject atkItem;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI atkLabel;
    [SerializeField] private GameObject magicItem;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI magicLabel;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI armorLabel;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI resistLabel;
    [SerializeField] private GameObject dodgeItem;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI dodgeLabel;
    [SerializeField] private GameObject aegisItem;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI aegisLabel;
    [SerializeField] private GameObject stealthItem;
    [SerializeField] private GameObject tauntItem;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI tauntLabel;

    public override void Init(Member m)
    {
        hpBar.Init(m);
        Set(cardPlaysItem, cardPlaysLabel, m.ExtraCardPlays());
        resourceGainItem.SetActive(false);
        Set(leadershipItem, leadershipLabel, m.Leadership());
        Set(atkItem, atkLabel, m.Attack());
        Set(magicItem, magicLabel, m.Magic());
        armorLabel.text = m.Armor().ToString();
        resistLabel.text = m.Resistance().ToString();
        Set(dodgeItem, dodgeLabel, m.Dodge());
        Set(aegisItem, aegisLabel, m.Aegis());
        stealthItem.SetActive(m.Stealth() > 0);
        Set(tauntItem, tauntLabel, m.Taunt());
    }

    public void SetPrimaryResourceGain(Sprite icon, int amount)
    {
        resourceGainIcon.sprite = icon;
        resourceGainLabel.text = amount.ToString();
        resourceGainItem.SetActive(true);
    }
    
    private void Set(GameObject g, TextMeshProUGUI t, int value)
    {
        g.SetActive(value > 0);
        t.text = value.ToString();
    }
}
