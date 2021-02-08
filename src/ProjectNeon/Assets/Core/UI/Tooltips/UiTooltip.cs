using UnityEngine;
using UnityEngine.EventSystems;

public class UiTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(1, 10), SerializeField] private string tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventData.dragging)
            Message.Publish(new ShowTooltip(tooltipText));
    }

    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}
