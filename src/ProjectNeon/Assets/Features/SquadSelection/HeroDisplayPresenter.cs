using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroDisplayPresenter : MonoBehaviour
{
    [SerializeField] private HeroCharacter currentHero;
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroClassName;
    [SerializeField] private TextMeshProUGUI heroDescription;
    [SerializeField] private TextMeshProUGUI roleDescription;
    [SerializeField] private TextMeshProUGUI backstory;
    [SerializeField] private TextMeshProUGUI heroSkill;
    [SerializeField] private Button showGrowthInfoButton;
    
    private void Start()
    {
        if (currentHero != null)
            Select(currentHero);
        showGrowthInfoButton.onClick.AddListener(ShowHeroPathway);
    }

    public void Select(HeroCharacter c)
    {
        currentHero = c;
        heroBust.sprite = c.Bust;
        heroName.text = c.Name;
        heroClassName.text = c.Class.Name;
        heroDescription.text = c.Flavor.HeroDescription;
        roleDescription.text = "Role: " + c.Flavor.RoleDescription;
        backstory.text = c.Flavor.BackStory;
        heroSkill.text = $"Skill: {c.Skills[0].SkillName.Value}";
    }

    private void ShowHeroPathway() => Message.Publish(new ShowHeroLevelUpPathway(currentHero));
    
}
