using UnityEngine;
using UnityEngine.EventSystems;

public class UiTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(1, 10), SerializeField] public string tooltipText;

    public void OnPointerEnter(PointerEventData eventData) => Show();

    public void OnPointerExit(PointerEventData eventData) => Hide();
    
    public void Show()
    {
        if (!MouseDragState.IsDragging)
            Message.Publish(new ShowTooltip(transform, tooltipText));
    }

    public void Hide() => Message.Publish(new HideTooltip());
}
