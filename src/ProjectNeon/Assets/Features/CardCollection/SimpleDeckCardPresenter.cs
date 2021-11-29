using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleDeckCardPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private HoverCard hoverCard;

    private Canvas _canvas;
    private Maybe<Card> _card;
    private CardTypeData _cardType;
    private int _count;
    private GameObject _hoverCard;

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
        Render();
        return this;
    }

    public SimpleDeckCardPresenter Initialized(int count, Card c)
    {
        _count = count;
        _card = c;
        _cardType = c.BaseType;
        Render();
        return this;
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
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hoverCard = Instantiate(hoverCard.gameObject, _canvas.transform);
        if (_card.IsPresent)
            _hoverCard.GetComponent<HoverCard>().Init(_card.Value);
        else
            _hoverCard.GetComponent<HoverCard>().Init(_cardType);
        Message.Publish(new CardHoveredOnDeck(transform));
    }

    public void OnPointerExit(PointerEventData eventData) => OnExit();
}