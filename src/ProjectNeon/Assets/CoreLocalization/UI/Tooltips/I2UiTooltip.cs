using UnityEngine;
using UnityEngine.EventSystems;

public class I2UiTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ILocalizeTerms
{
    [SerializeField] public string tooltipTerm;
    [TextArea(1, 10), SerializeField] public string tooltipText;

    public void OnPointerEnter(PointerEventData eventData) => Show();

    public void OnPointerExit(PointerEventData eventData) => Hide();

    public void Show()
    {
        if (!MouseDragState.IsDragging && tooltipTerm != null)
            Message.Publish(new ShowTooltip(transform, tooltipTerm.ToLocalized()));
    }

    public void Hide() => Message.Publish(new HideTooltip());

    public string[] GetLocalizeTerms()
        => new[] {tooltipTerm};
}