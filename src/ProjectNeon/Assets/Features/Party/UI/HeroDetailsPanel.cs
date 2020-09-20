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
    [SerializeField] private TextCommandButton levelUpButton;

    public HeroDetailsPanel Initialized(Hero h, Member m)
    {
        nameLabel.text = h.Name;
        classLabel.text = h.Class.Name;
        heroBust.sprite = h.Character.Bust;
        stats.Initialized(m);
        if (equipment != null)
            equipment.Initialized(h);
        
        levelUpButton?.gameObject.SetActive(false);
        if (levelUpButton != null && h.LevelUpPoints > 0)
            levelUpButton.Init("Level Up", () => Message.Publish(new LevelUpHero(h)));
        
        return this;
    }
}
