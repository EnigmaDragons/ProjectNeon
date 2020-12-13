using UnityEngine;
using UnityEngine.EventSystems;

public class UiTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string tooltipText;

    public void OnPointerEnter(PointerEventData eventData) => Message.Publish(new ShowTooltip(tooltipText));
    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}
