using TMPro;
using UnityEngine;

public class MemberSimplifiedVisualStatPanel : MemberUiBase
{
    [SerializeField] private UIHPBarController hpBar;
    [SerializeField] private GameObject atkItem;
    [SerializeField] private TextMeshProUGUI atkLabel;
    [SerializeField] private GameObject magicItem;
    [SerializeField] private TextMeshProUGUI magicLabel;
    [SerializeField] private TextMeshProUGUI armorLabel;
    [SerializeField] private TextMeshProUGUI resistLabel;
    [SerializeField] private GameObject dodgeItem;
    [SerializeField] private TextMeshProUGUI dodgeLabel;
    [SerializeField] private GameObject aegisItem;
    [SerializeField] private TextMeshProUGUI aegisLabel;
    [SerializeField] private GameObject stealthItem;
    
    public override void Init(Member m)
    {
        hpBar.Init(m);
        Set(atkItem, atkLabel, m.Attack());
        Set(magicItem, magicLabel, m.Magic());
        armorLabel.text = m.Armor().ToString();
        resistLabel.text = m.Resistance().ToString();
        Set(dodgeItem, dodgeLabel, m.Dodge());
        Set(aegisItem, aegisLabel, m.Aegis());
        stealthItem.SetActive(m.Stealth() > 0);
    }

    private void Set(GameObject g, TextMeshProUGUI t, int value)
    {
        g.SetActive(value > 0);
        t.text = value.ToString();
    }
}
