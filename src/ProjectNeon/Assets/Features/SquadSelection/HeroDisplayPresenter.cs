using System;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroDisplayPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private BaseHero currentHero;
    [SerializeField] private GameObject hoverGraphic;
    [SerializeField] private Image heroBust;
    [SerializeField] private Localize heroName;
    [SerializeField] private Localize heroClassName;
    [SerializeField] private Localize heroDescription;
    [FormerlySerializedAs("roleDescription")] [SerializeField] private Localize complexityLabel;
    [SerializeField] private Slider complexitySlider;
    [SerializeField] private Localize backstory;
    [SerializeField] private NamedGameObject[] tabTargets;
    [SerializeField] private NamedGameObject[] tabGlowTargets;
    [SerializeField] private ResourceCounterPresenter resource1;
    [SerializeField] private ResourceCounterPresenter resource2;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI startingCredits;
    [SerializeField] private MemberSimplifiedVisualStatPanel statPanel;
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private Button statButton;
    [SerializeField] private Button loreButton;
    [SerializeField] private Button cardsButton;
    [SerializeField] private SimpleDeckUI deckUi;
    [SerializeField] private SimpleDeckUI basicUi;
    [SerializeField] private ResourceSpriteMap resourceIcons;
    [SerializeField] private GameObject controlsReference;
    [SerializeField] private Localize confirmLabel;
    [SerializeField] private ConfirmActionComponent confirm;
    [SerializeField] private AlternateActionComponent changeTabs;

    private bool _isInitialized;
    private bool _isClickable = false;
    private Action _onClick = () => { };
    private string _tab;
    
    private void Start()
    {
        statButton.onClick.AddListener(() => ShowTab("Stats"));
        loreButton.onClick.AddListener(() => ShowTab("Lore"));
        cardsButton.onClick.AddListener(() => ShowTab("Cards"));
        if (buttonsPanel.activeSelf)
            ShowTab("Lore");
        confirm.Bind(Confirm);
        changeTabs.Bind(NextTab);
        if (!_isInitialized && currentHero != null)
            Init(currentHero);
    }

    public void Hide() => gameObject.SetActive(false);

    public void DisableClick()
    {
        _isClickable = false;
        controlsReference.SetActive(false);
    }

    public void SetControlText(string term) => confirmLabel.SetTerm(term);
    public void Init(BaseHero c) => Init(c, false, () => {});
    public void Init(BaseHero c, bool isClickable, Action onClick) => Init(c, c.AsMemberForLibrary(), isClickable, onClick);
    public void Init(BaseHero c, Member m, bool isClickable, Action onClick)
    {
        _isInitialized = true;
        _onClick = onClick;
        _isClickable = isClickable;
        if (!_isClickable)
            controlsReference.SetActive(false);
        currentHero = c;
        heroBust.sprite = c.Bust;
        heroName.SetTerm(c.NameTerm());
        heroClassName.SetTerm(c.ClassTerm());
        heroDescription.SetTerm(c.DescriptionTerm());
        complexityLabel.SetFinalText($"{"Menu/Complexity".ToLocalized()}:");
        complexitySlider.value = Mathf.Clamp(c.ComplexityRating / 5f, 0.2f, 1f);
        backstory.SetTerm(c.BackStoryTerm());
        var member = m;
        if (statPanel != null)
        {
            statPanel.Init(member);
            var resourceGains = c.TurnResourceGains;
            if (resourceGains.Length > 0)
                if (resourceGains[0].BaseAmount > 0)
                    statPanel.SetPrimaryResourceGain(resourceIcons.Get(resourceGains[0].ResourceType.Name), resourceGains[0].BaseAmount);
        }
        if (deckUi != null)
            deckUi.Init(c.Deck.Cards.Select(card => card.ToNonBattleCard(c, c.Stats)).ToArray());
        if (basicUi != null)
            basicUi.Init(c.BasicCard.ToNonBattleCard(c, c.Stats).AsArray(), false);
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

        if (startingCredits != null)
            startingCredits.text = c.StartingCredits.ToString();
    }

    public void LockToTab(string tabName)
    {
        ShowTab(tabName);
        buttonsPanel.SetActive(false);
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
        if (_isClickable && eventData.button == PointerEventData.InputButton.Left)
            _onClick();
    }

    public void Confirm()
    {
        if (_isClickable)
            _onClick();
    } 

    private void NextTab()
    {
        if (_tab == "Lore")
            ShowTab("Cards");
        else if (_tab == "Cards")
            ShowTab("Stats");
        else if (_tab == "Stats")
            ShowTab("Lore");
    }

    private void ShowTab(string tabName)
    {
        _tab = tabName;
        foreach (var tab in tabTargets) 
            tab.Obj.SetActive(tab.Name.Equals(_tab));
        foreach (var glow in tabGlowTargets) 
            glow.Obj.SetActive(glow.Name.Equals(_tab));
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_isClickable && hoverGraphic != null)
            hoverGraphic.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_isClickable && hoverGraphic != null)
            hoverGraphic.SetActive(false);
    }
}
