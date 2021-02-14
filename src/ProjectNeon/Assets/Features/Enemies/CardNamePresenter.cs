using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardNamePresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private HoverCard hoverCard;

    private Canvas _canvas;
    private CardType _card;
    private GameObject _hoverCard;
    
    private void Awake() => _canvas = FindObjectOfType<Canvas>();
    private void OnDestroy() => OnExit();
    
    public void Init(CardType card)
    {
        _card = card;
        UpdateInfo();
    }

    public void OnHover()
    {
        _hoverCard = Instantiate(hoverCard.gameObject, _canvas.transform);
        _hoverCard.GetComponent<HoverCard>().Init(_card);
    }

    public void OnExit()
    {
        if (_hoverCard != null)
            Destroy(_hoverCard);
    }

    private void UpdateInfo() => cardNameText.text = _card.Name;

    public void OnPointerEnter(PointerEventData eventData) => OnHover();

    public void OnPointerExit(PointerEventData eventData) => OnExit();

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            _card.ShowDetailedCardView();
    }
}
