using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardNamePresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private GameObject hoverCardArea;

    private CardType _card;
    private Maybe<Member> _owner;

    private void OnDisable() => OnExit();
    
    public void Init(CardType card, Maybe<Member> owner)
    {
        _card = card;
        _owner = owner;
        UpdateInfo();
    }

    private void UpdateInfo() => cardNameText.text = _card.Name;

    public void OnPointerEnter(PointerEventData eventData) => OnHover();
    public void OnPointerExit(PointerEventData eventData) => OnExit();
    public void OnHover() => Message.Publish(
        new ShowChainedCard(hoverCardArea, _owner.Select(o => _card.CreateInstance(o.Id, o), Maybe<Card>.Missing), _card));
    public void OnExit() => Message.Publish(new HideChainedCard());
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            _card.ShowDetailedCardView();
    }
}
