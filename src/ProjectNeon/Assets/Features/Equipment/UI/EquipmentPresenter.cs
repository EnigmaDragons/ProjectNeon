using System;
using System.Linq;
using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentPresenter : OnMessage<LanguageChanged>, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, ILocalizeTerms, ISelectHandler, IDeselectHandler
{
    [SerializeField] private SmoothFocusDarken focusDarken;
    [SerializeField] private GameObject highlight;
    [SerializeField] private Localize nameLabel;
    [SerializeField] private Localize descriptionLabel;
    [SerializeField] private Localize classesLabel;
    [SerializeField] private RarityPresenter rarity;
    [SerializeField] private Image slotIcon;
    [SerializeField] private EquipmentSlotIcons slotIcons;
    [SerializeField] private CorpUiBase corpBranding;
    [SerializeField] private AllCorps allCorps;
    [SerializeField] private GearRulesPresenter rulesPresenter;
    [SerializeField] private LocalizedLabelPresenter corpLabel;
    [SerializeField] private HoverCard hoverCardPrototype;
    [SerializeField] private ConfirmActionComponent confirm;

    private static void NoOp() {}
    
    private bool _useHoverHighlight = false;
    private bool _useAnyHover = false;
    private bool _useDarkenOnHover = true;
    private Action _onClick = NoOp;
    private Action _onHoverEnter = NoOp;
    private Action _onHoverExit = NoOp;
    private StaticEquipment _currentEquipment;
    
    private Maybe<CardType> _referencedCard = Maybe<CardType>.Missing();
    private Canvas _canvas;
    private HoverCard _hoverCard;
    
    private const string ArchetypesTerm = "Archetypes/Archetypes";
    private const string AnyTerm = "Archetypes/Any";
    private const string MadeByTerm = "BattleUI/Made By";

    private void Awake() => confirm.Bind(() => _onClick());
    
    public void Set(StaticEquipment e, Action onClick) => Initialized(e, onClick);
    
    public EquipmentPresenter Initialized(StaticEquipment e, Action onClick, bool useHoverHighlight = false, bool useAnyHover = true)
    {
        InitCanvasIfNeeded();
        ClearHoverCard();
        _currentEquipment = e;
        _onClick = onClick;
        _onHoverEnter = NoOp;
        _onHoverExit = NoOp;
        _useHoverHighlight = useHoverHighlight;
        _useAnyHover = useAnyHover;
        _useDarkenOnHover = true;
        if (nameLabel != null)
            nameLabel.SetTerm(e.LocalizationNameTerm());
        var archetypeText = e.Archetypes.Any()
            ? e.LocalizedArchetypeDescription()
            : AnyTerm.ToLocalized();
        var labelPrefix = ArchetypesTerm.ToLocalized();
        classesLabel.SetFinalText($"{labelPrefix} - {archetypeText}");
        var description = ResourceIcons.ReplaceTextWithResourceIcons(e.LocalizationDescriptionTerm().ToLocalized());
        descriptionLabel.SetFinalText(description);
        rarity.Set(e.Rarity);
        slotIcon.sprite = slotIcons.All[e.Slot];
        var corp = allCorps.GetCorpByNameOrNone(e.Corp);
        corpBranding.Init(corp);
        corpLabel.Label.SetFinalText($"{MadeByTerm.ToLocalized()} {corp.GetLocalizedName()}");

        highlight.SetActive(false);
        rulesPresenter.Hide();
        corpLabel.Hide();
        focusDarken.Hide();

        _referencedCard = _currentEquipment.ReferencedCard;

        gameObject.SetActive(true);
        return this;
    }

    public EquipmentPresenter WithHoverSettings(bool useAnyHover, bool useHoverHighlight, bool useHoverDarken)
    {
        _useAnyHover = useAnyHover;
        _useHoverHighlight = useHoverHighlight;
        _useDarkenOnHover = useHoverDarken;
        return this;
    }

    protected override void AfterDisable() => ClearHoverCard();
    
    protected override void Execute(LanguageChanged msg)
    {
        if (_currentEquipment != null)
            Initialized(_currentEquipment, _onClick, _useHoverHighlight, _useAnyHover);
    }

    public void SetOnHover(Action onHoverEnter, Action onHoverExit)
    {
        _onHoverEnter = onHoverEnter;
        _onHoverExit = onHoverExit;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            _onClick();
    }

    public void OnSelect(BaseEventData eventData) => OnPointerEnter(new PointerEventData(EventSystem.current));
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_useAnyHover)
            return;
        
        if (_useHoverHighlight)
            highlight.SetActive(true);
        
        if (_useDarkenOnHover)
            focusDarken.Show();
        
        _onHoverEnter();
        rulesPresenter.Show(_currentEquipment);
        corpLabel.Show();
        Message.Publish(new ItemHovered(transform));
        InitCanvasIfNeeded();
        if (_canvas != null)
            _referencedCard.IfPresent(c => _hoverCard = Instantiate(hoverCardPrototype, _canvas.transform).Initialized(c));
    }
    
    public void OnDeselect(BaseEventData eventData) => OnPointerExit(new PointerEventData(EventSystem.current));
    public void OnPointerExit(PointerEventData eventData)
    {        
        if (!_useAnyHover)
            return;
        
        if (_useHoverHighlight)
            highlight.SetActive(false);
        
        _onHoverExit();
        rulesPresenter.Hide();
        corpLabel.Hide();
        focusDarken.Hide();
        ClearHoverCard();
    }
    
    public void SetReferencedCardCanvas(Canvas c)
    {
        _canvas = c;
    }

    private void ClearHoverCard()
    {
        if (_hoverCard != null)
        {
            DestroyImmediate(_hoverCard.gameObject);
            _hoverCard = null;
        }
    }
    
    private void InitCanvasIfNeeded()
    {
        if (_canvas != null)
            return;
        
        var allCanvases = FindObjectsOfType<Canvas>();
        _canvas = allCanvases
            .Where(c => c != null)
            .Where(c => c.gameObject.activeInHierarchy)
            .OrderByDescending(c => c.sortingOrder)
            .FirstOrDefault();
    }

    public string[] GetLocalizeTerms() => new[] { MadeByTerm, ArchetypesTerm, AnyTerm, $"Archetypes/General" };
}
