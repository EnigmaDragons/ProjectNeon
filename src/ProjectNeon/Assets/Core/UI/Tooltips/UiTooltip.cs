using UnityEngine;
using UnityEngine.EventSystems;

public class UiTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(1, 10), SerializeField] public string tooltipText;

    public void OnPointerEnter(PointerEventData eventData) => Show();

    public void OnPointerExit(PointerEventData eventData) => Hide();
    
    public void Show()
    {
        if (!MouseDragState.IsDragging && tooltipText != null && gameObject.activeSelf)
            Message.Publish(new ShowTooltip(transform.position, tooltipText));
    }

    public void Hide() => Message.Publish(new HideTooltip());
}
