using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = System.Object;

public class EquipmentPresenter : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SmoothFocusDarken focusDarken;
    [SerializeField] private GameObject highlight;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI slotLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;
    [SerializeField] private TextMeshProUGUI classesLabel;
    [SerializeField] private RarityPresenter rarity;
    [SerializeField] private Image slotIcon;
    [SerializeField] private EquipmentSlotIcons slotIcons;
    [SerializeField] private CorpUiBase corpBranding;
    [SerializeField] private AllCorps allCorps;
    [SerializeField] private GearRulesPresenter rulesPresenter;
    [SerializeField] private LabelPresenter corpLabel;
    [SerializeField] private HoverCard hoverCardPrototype;

    private static void NoOp() {}
    
    private bool _useHoverHighlight = false;
    private bool _useAnyHover = false;
    private bool _useDarkenOnHover = true;
    private Action _onClick = NoOp;
    private Action _onHoverEnter = NoOp;
    private Action _onHoverExit = NoOp;
    private Equipment _currentEquipment;
    
    private Maybe<CardTypeData> _referencedCard = Maybe<CardTypeData>.Missing();
    private Canvas _canvas;
    private HoverCard _hoverCard;

    public void Set(Equipment e, Action onClick) => Initialized(e, onClick);
    
    public EquipmentPresenter Initialized(Equipment e, Action onClick, bool useHoverHighlight = false, bool useAnyHover = true)
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
        nameLabel.text = e.Name;
        slotLabel.text = $"{e.Slot}";
        var archetypeText = e.Archetypes.Any()
            ? string.Join(",", e.Archetypes.Select(c => c))
            : "Any";
        classesLabel.text = $"Archetypes: {archetypeText}";
        descriptionLabel.text = e.GetInterpolatedDescription();
        rarity.Set(e.Rarity);
        slotIcon.sprite = slotIcons.All[e.Slot];
        var corp = allCorps.GetCorpByNameOrNone(e.Corp);
        corpBranding.Init(corp);
        corpLabel.Init($"Made By {corp.Name}");
        
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

    private void OnDisable() => ClearHoverCard();

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
}
