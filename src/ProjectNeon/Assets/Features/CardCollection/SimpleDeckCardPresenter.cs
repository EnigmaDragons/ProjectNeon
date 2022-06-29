using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleDeckCardPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image cardArt;
    [SerializeField] private HoverCard hoverCard;
    [SerializeField] private CardCostPresenter costPresenter;
    [SerializeField] private Image archetypeTint;
    [SerializeField] private ArchetypeTints tints;

    private Canvas _canvas;
    private Maybe<Card> _card = Maybe<Card>.Missing();
    private CardTypeData _cardType;
    private int _count;
    private GameObject _hoverCard;
    private Action _leftClickAction = () => { };

    private void Awake() => InitCanvasIfNeeded();
    private void OnDestroy() => OnExit();

    public void SetCanvas(Canvas c)
    {
        _canvas = c;
    }
    
    public SimpleDeckCardPresenter Initialized(int count, CardTypeData c)
    {
        _count = count;
        _card = Maybe<Card>.Missing();
        _cardType = c;
        _leftClickAction = () => { };
        Render();
        return this;
    }

    public SimpleDeckCardPresenter Initialized(int count, Card c)
    {
        _count = count;
        _card = c;
        _cardType = c.BaseType;
        _leftClickAction = () => { };
        Render();
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
        cardNameText.text = _cardType.Name;
        countText.text = _count > -1 ? _count.ToString() : "";
        if (cardArt != null)
        {
            cardArt.gameObject.SetActive(true);
            cardArt.sprite = _cardType.Art;
        }
        if (costPresenter != null)
        {
            costPresenter.Render(_card, _cardType, _card.Select(c => c.Owner.PrimaryResourceType(), () => _cardType.Cost.ResourceType), forceShowXcostAsX: true);
        }
        if (archetypeTint != null)
            archetypeTint.color = tints.ForArchetypes(_cardType.Archetypes).WithAlpha(0.75f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_cardType == null)
            return;
        
        _hoverCard = Instantiate(hoverCard.gameObject, _canvas.transform);
        if (_card.IsPresent)
            _hoverCard.GetComponent<HoverCard>().Init(_card.Value);
        else
            _hoverCard.GetComponent<HoverCard>().Init(_cardType);
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
}