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
    
    private void Start()
    {
        if (currentHero != null)
            Select(currentHero);
    }

    public void Select(HeroCharacter c)
    {
        currentHero = c;
        heroBust.sprite = c.Bust;
        heroName.text = c.DisplayName();
        heroClassName.text = c.Class;
        heroDescription.text = c.Flavor.HeroDescription;
        roleDescription.text = "Role: " + c.Flavor.RoleDescription;
        backstory.text = c.Flavor.BackStory;
    }

    private void ShowHeroPathway() => Message.Publish(new ShowHeroLevelUpPathway(currentHero));
    
}
