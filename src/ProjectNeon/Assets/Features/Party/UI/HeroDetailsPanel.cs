using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HeroDetailsPanel : MonoBehaviour
{
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI classLabel;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private HeroEquipmentPanel equipment;

    public HeroDetailsPanel Initialized(Hero h, Member m)
    {
        nameLabel.text = h.Character.Name;
        classLabel.text = h.Character.Class.Name;
        heroBust.sprite = h.Character.Bust;
        stats.Initialized(m);
        if (equipment != null)
            equipment.Initialized(h);
        return this;
    }
}
