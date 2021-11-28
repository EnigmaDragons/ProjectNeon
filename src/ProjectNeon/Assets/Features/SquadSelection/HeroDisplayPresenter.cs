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
    [SerializeField] private NamedGameObject[] tabTargets;
    [SerializeField] private MemberSimplifiedVisualStatPanel statPanel;
    [SerializeField] private Button statButton;
    [SerializeField] private Button loreButton;
    [SerializeField] private Button cardsButton;

    private bool _isInitialized;
    private bool _isClickable = false;
    private Action _onClick = () => { };
    
    private void Start()
    {
        statButton.onClick.AddListener(() => ShowTab("Stats"));
        loreButton.onClick.AddListener(() => ShowTab("Lore"));
        cardsButton.onClick.AddListener(() => ShowTab("Cards"));
        loreButton.Select();
        if (!_isInitialized && currentHero != null)
            Init(currentHero);
    }

    public void Init(HeroCharacter c) => Init(c, false, () => {});
    public void Init(HeroCharacter c, bool isClickable, Action onClick)
    {
        _isInitialized = true;
        _onClick = onClick;
        _isClickable = isClickable;
        currentHero = c;
        heroBust.sprite = c.Bust;
        heroName.text = c.DisplayName();
        heroClassName.text = c.Class;
        heroDescription.text = c.Flavor.HeroDescription;
        roleDescription.text = "Role: " + c.Flavor.RoleDescription;
        backstory.text = c.Flavor.BackStory;
        var member = c.AsMemberForLibrary();
        if (statPanel != null)
            statPanel.Init(member);
    }

    private void ShowHeroPathway() => Message.Publish(new ShowHeroLevelUpPathway(currentHero));

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isClickable && hoverGraphic != null)
            hoverGraphic.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isClickable && hoverGraphic != null)
            hoverGraphic.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) => _onClick();

    private void ShowTab(string tabName)
    {
        foreach (var tab in tabTargets) 
            tab.Obj.SetActive(tab.Name.Equals(tabName));
    }
}
