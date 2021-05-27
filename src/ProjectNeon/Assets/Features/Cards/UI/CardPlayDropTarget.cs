using UnityEngine;
using UnityEngine.EventSystems;

public sealed class CardPlayDropTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var cardComponent = eventData.pointerDrag.GetComponent<CardPresenter>();
        if (cardComponent != null)
        {
            cardComponent.Activate();
            eventData.pointerDrag = null;
        }
    }
}

