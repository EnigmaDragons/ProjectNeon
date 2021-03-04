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
    [SerializeField] private HeroInjuryPanel injuries;
    [SerializeField] private TextCommandButton levelUpButton;

    public HeroDetailsPanel Initialized(Hero h, bool canInteractWithEquipment)
    {
        nameLabel.text = h.Name;
        classLabel.text = h.Class.Name;
        heroBust.sprite = h.Character.Bust;
        stats.Initialized(h);
        injuries.Init(h);
        if (equipment != null)
            equipment.Initialized(h, !canInteractWithEquipment);
        
        levelUpButton?.gameObject.SetActive(false);
        if (levelUpButton != null && h.Levels.LevelUpPoints > 0)
            levelUpButton.Init("Level Up", () => Message.Publish(new LevelUpHero(h)));
        
        return this;
    }
}
