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
        nameLabel.text = h.BaseHero.Name;
        classLabel.text = h.BaseHero.Class.Name;
        heroBust.sprite = h.BaseHero.Bust;
        stats.Initialized(m);
        equipment.Initialized(h);
        return this;
    }
}
