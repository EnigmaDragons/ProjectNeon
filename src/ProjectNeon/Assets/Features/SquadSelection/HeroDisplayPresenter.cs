using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroDisplayPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private HeroCharacter currentHero;
    [SerializeField] private GameObject hoverGraphic;
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroClassName;
    [SerializeField] private TextMeshProUGUI heroDescription;
    [SerializeField] private TextMeshProUGUI roleDescription;
    [SerializeField] private TextMeshProUGUI backstory;

    private Maybe<Action> _onClick = Maybe<Action>.Missing();
    
    private void Start()
    {
        if (currentHero != null)
            Init(currentHero);
    }

    public void Init(HeroCharacter c) => Init(c, Maybe<Action>.Missing());
    public void Init(HeroCharacter c, Maybe<Action> onClick)
    {
        _onClick = onClick;
        currentHero = c;
        heroBust.sprite = c.Bust;
        heroName.text = c.DisplayName();
        heroClassName.text = c.Class;
        heroDescription.text = c.Flavor.HeroDescription;
        roleDescription.text = "Role: " + c.Flavor.RoleDescription;
        backstory.text = c.Flavor.BackStory;
    }

    private void ShowHeroPathway() => Message.Publish(new ShowHeroLevelUpPathway(currentHero));

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_onClick.IsPresent && hoverGraphic != null)
            hoverGraphic.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_onClick.IsPresent && hoverGraphic != null)
            hoverGraphic.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) => _onClick.IfPresent(a => a());
}
