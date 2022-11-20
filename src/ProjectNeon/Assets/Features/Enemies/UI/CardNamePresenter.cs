using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardNamePresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private Localize cardNameText;
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

    private void UpdateInfo() => cardNameText.SetTerm(_card.LocalizationNameTerm());

    public void OnPointerEnter(PointerEventData eventData) => OnHover();
    public void OnPointerExit(PointerEventData eventData) => OnExit();
    public void OnHover() => Message.Publish(
        new ShowReferencedCard(hoverCardArea, _owner.Select(o => _card.CreateInstance(o.Id, o), Maybe<Card>.Missing), _card));
    public void OnExit() => Message.Publish(new HideReferencedCard());
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) 
            return;
        
        if (_owner.IsPresent)
            new Card(-1, _owner.Value, _card).ShowDetailedCardView();
        else
            _card.ShowDetailedCardView();
    }
}
