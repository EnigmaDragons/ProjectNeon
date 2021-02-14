using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardNamePresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private GameObject hoverCardArea;

    private CardType _card;

    private void OnDisable() => OnExit();
    
    public void Init(CardType card)
    {
        _card = card;
        UpdateInfo();
    }

    private void UpdateInfo() => cardNameText.text = _card.Name;

    public void OnPointerEnter(PointerEventData eventData) => OnHover();
    public void OnPointerExit(PointerEventData eventData) => OnExit();
    public void OnHover() => Message.Publish(new ShowChainedCard(hoverCardArea, _card));
    public void OnExit() => Message.Publish(new HideChainedCard());
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            _card.ShowDetailedCardView();
    }
}
