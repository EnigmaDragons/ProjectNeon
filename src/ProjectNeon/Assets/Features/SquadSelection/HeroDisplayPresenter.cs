using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroDisplayPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private HeroCharacter currentHero;
    [SerializeField] private GameObject hoverGraphic;
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroClassName;
    [SerializeField] private TextMeshProUGUI heroDescription;
    [FormerlySerializedAs("roleDescription")] [SerializeField] private TextMeshProUGUI complexityLabel;
    [SerializeField] private TextMeshProUGUI backstory;
    [SerializeField] private NamedGameObject[] tabTargets;
    [SerializeField] private ResourceCounterPresenter resource1;
    [SerializeField] private ResourceCounterPresenter resource2;
    [SerializeField] private MemberSimplifiedVisualStatPanel statPanel;
    [SerializeField] private Button statButton;
    [SerializeField] private Button loreButton;
    [SerializeField] private Button cardsButton;
    [SerializeField] private SimpleDeckUI deckUi;
    [SerializeField] private SimpleDeckUI basicUi;

    private bool _isInitialized;
    private bool _isClickable = false;
    private Action _onClick = () => { };
    
    private void Start()
    {
        statButton.onClick.AddListener(() => ShowTab("Stats"));
        loreButton.onClick.AddListener(() => ShowTab("Lore"));
        cardsButton.onClick.AddListener(() => ShowTab("Cards"));
        loreButton.Select();
        ShowTab("Lore");
        if (!_isInitialized && currentHero != null)
            Init(currentHero);
    }

    public void Hide() => gameObject.SetActive(false);
    
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
        complexityLabel.text = $"Complexity: {c.ComplexityRating}/5";
        backstory.text = c.Flavor.BackStory;
        var member = c.AsMemberForLibrary();
        if (statPanel != null)
        {
            statPanel.Init(member);
            var resourceGains = c.TurnResourceGains;
            if (resourceGains.Length > 0)
                if (resourceGains[0].BaseAmount > 0)
                    statPanel.SetPrimaryResourceGain(resourceGains[0].ResourceType.Icon, resourceGains[0].BaseAmount);
        }
        if (deckUi != null)
            deckUi.Init(c.Deck.Cards.Select(card => card.ToNonBattleCard(c)).ToArray());
        if (basicUi != null)
            basicUi.Init(c.BasicCard.ToNonBattleCard(c).AsArray(), false);
        if (resource1 != null)
        {
            resource1.SetReactivity(false);
            if(member.State.ResourceTypes.Length < 1)
                resource1.Hide();
            else
                resource1.Init(member, member.State.ResourceTypes[0]);
        }
        if (resource2 != null)
        { 
            resource2.SetReactivity(false);
            if(member.State.ResourceTypes.Length < 2)
                resource2.Hide();
            else
                resource2.Init(member, member.State.ResourceTypes[1]);
        }
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            _onClick();
    }

    private void ShowTab(string tabName)
    {
        foreach (var tab in tabTargets) 
            tab.Obj.SetActive(tab.Name.Equals(tabName));
    }
}
