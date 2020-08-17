using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HeroDetailsPanel : MonoBehaviour
{
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI classLabel;
    [SerializeField] private MemberStatPanel stats;

    public HeroDetailsPanel Initialized(Hero h, Member m)
    {
        nameLabel.text = h.Name;
        classLabel.text = h.Class.Name;
        heroBust.sprite = h.Bust;
        stats.Initialized(m);
        return this;
    }
}
