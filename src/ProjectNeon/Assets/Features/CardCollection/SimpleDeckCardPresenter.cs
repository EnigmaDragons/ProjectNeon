using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleDeckCardPresenter : OnMessage<SceneChanged>, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Localize cardNameText;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI countText;
    [SerializeField] private Image cardArt;
    [SerializeField] private HoverCard hoverCard;
    [SerializeField] private CardCostPresenter costPresenter;
    [SerializeField] private Image archetypeTint;
    [SerializeField] private UnityEngine.UI.Extensions.Gradient tintGradient;
    [SerializeField] private ArchetypeTints tints;
    [SerializeField] private ConfirmActionComponent confirm;
    [SerializeField] private InspectActionComponent inspect;

    private Canvas _canvas;
    private Maybe<Card> _card = Maybe<Card>.Missing();
    private CardType _cardType;
    private int _count;
    private GameObject _hoverCard;
    private Action _leftClickAction = () => { };
    private bool _isBasic;
    private bool _isSelected;

    private void Awake()
    {
        confirm.Bind(() => _leftClickAction());
        inspect.Bind(() =>
        {
            if (_card.IsPresent)
                Message.Publish(new ShowDetailedCardView(_card.Value));
        });
        InitCanvasIfNeeded();
    }

    protected override void AfterEnable() => InitCanvasIfNeeded();

    private void OnDisable() => OnExit();

    protected override void Execute(SceneChanged msg) => OnExit();

    private void OnDestroy() => OnExit();

    public void SetCanvas(Canvas c)
    {
        _canvas = c;
    }
    
    public SimpleDeckCardPresenter Initialized(int count, CardType c)
    {
        _count = count;
        _card = Maybe<Card>.Missing();
        _cardType = c;
        _leftClickAction = () => { };
        _isBasic = false;
        Render();
        if (_isSelected)
        {
            OnDeselect(null);
            OnSelect(null);
        }
        return this;
    }

    public SimpleDeckCardPresenter Initialized(int count, Card c)
    {
        _count = count;
        _card = c;
        _cardType = c.CardTypeOrNothing.Value;
        _leftClickAction = () => { };
        _isBasic = c.Owner.BasicCard.IsPresentAnd(b => c.Id == b.Id);
        Render();
        if (_isSelected)
        {
            OnDeselect(null);
            OnSelect(null);
        }
        return this;
    }

    public void BindLeftClickAction(Action a)
    {
        _leftClickAction = a;
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
            .First();
    }
    
    private void OnExit()
    {
        if (_hoverCard != null)
            Destroy(_hoverCard);
    }

    private void Render()
    {
        if (_cardType == null)
            return;
        cardNameText.SetTerm(_cardType.NameTerm);
        countText.text = _isBasic 
            ? "B" 
            : _count > -1 
                ? _count.ToString() 
                : string.Empty;
        if (cardArt != null)
        {
            cardArt.gameObject.SetActive(true);
            cardArt.sprite = _cardType.Art;
        }
        if (costPresenter != null)
        {
            costPresenter.Render(_card, _cardType, _card.Select(c => c.Owner.PrimaryResourceType(), () => _cardType.Cost.ResourceType), forceShowXcostAsX: true);
        }
        RenderTint();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_cardType == null || !gameObject.activeSelf)
            return;

        InitCanvasIfNeeded();
        _hoverCard = Instantiate(hoverCard.gameObject, _canvas.transform);
        if (_card.IsPresent)
            _hoverCard.GetComponent<HoverCard>().Init(_card.Value, true);
        else
            _hoverCard.GetComponent<HoverCard>().Init(_cardType, true);
        Message.Publish(new CardHoveredOnDeck(transform));
    }

    public void OnPointerExit(PointerEventData eventData) => OnExit();
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            _leftClickAction();
        
        if (eventData.button == PointerEventData.InputButton.Right)
            if (_card.IsPresent)
                Message.Publish(new ShowDetailedCardView(_card.Value));
    }
    
    private void RenderTint()
    {
        if (archetypeTint != null)
            archetypeTint.color = tints.ForArchetypes(_cardType.Archetypes).WithAlpha(0.75f);
        tintGradient.enabled = _cardType.Archetypes.Count > 1;
        if (_cardType.Archetypes.Count == 0)
            SetGradientColors(Color.white, Color.white);
        else if (_cardType.Archetypes.Count == 1)
        {
            var t = tints.ForArchetypes(_cardType.Archetypes);
            SetGradientColors(t, t);
        }
        else
            SetGradientColors(
                tints.ForArchetypes(new HashSet<string>(new[] {_cardType.Archetypes.OrderBy(x => x).First()})),
                tints.ForArchetypes(new HashSet<string>(new[] {_cardType.Archetypes.OrderBy(x => x).Last()})));
    }
    
    private void SetGradientColors(Color cOne, Color cTwo)
    {
        tintGradient.Vertex1 = cTwo;
        tintGradient.Vertex2 = cOne;
    }

    public void DisableInteractions()
    {
        _leftClickAction = () => { };
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_cardType == null || !gameObject.activeSelf)
            return;

        _isSelected = true;
        InitCanvasIfNeeded();
        _hoverCard = Instantiate(hoverCard.gameObject, _canvas.transform);
        var position = transform.position;
        _hoverCard.transform.position = new Vector3(position.x + 250, position.y, position.z);
        if (_card.IsPresent)
            _hoverCard.GetComponent<HoverCard>().Init(_card.Value, false);
        else
            _hoverCard.GetComponent<HoverCard>().Init(_cardType, false);
        Message.Publish(new CardHoveredOnDeck(transform));
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
        OnExit();
    }
}