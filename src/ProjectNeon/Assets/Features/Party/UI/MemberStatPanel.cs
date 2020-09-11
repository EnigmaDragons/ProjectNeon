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

    public MemberStatPanel Initialized(Member m)
    {
        hpLabel.text = $"{m.CurrentHp()}/{m.MaxHp()}";
        maxShieldLabel.text = $"{m.MaxShield()}";
        attackLabel.text = $"{m.Attack()}";
        magicLabel.text = $"{m.Magic()}";
        armorLabel.text = $"{m.Armor()}";
        resistLabel.text = $"{m.Resistance()}";
        toughLabel.text = $"{m.Toughness()}";
        
        return this;
    }
}
