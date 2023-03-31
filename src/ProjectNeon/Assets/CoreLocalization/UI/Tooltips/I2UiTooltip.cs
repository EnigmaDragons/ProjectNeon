using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class I2UiTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ILocalizeTerms
{
    [SerializeField] public string tooltipTerm;

    private bool _showing = false;

    private void OnDisable() => Hide();
    private void OnDestroy() => Hide();

    public void OnPointerEnter(PointerEventData eventData) => Show();

    public void OnPointerExit(PointerEventData eventData) => Hide();

    public void Show()
    {
        if (!MouseDragState.IsDragging && tooltipTerm != null && gameObject.activeSelf)
        {
            _showing = true;
            Message.Publish(new ShowTooltip(transform.position, tooltipTerm.ToLocalized()));
        }
    }

    public void Hide()
    {
        if (_showing)
        {
            _showing = false;
            Message.Publish(new HideTooltip());
        }
    }

    public string[] GetLocalizeTerms()
        => new[] {tooltipTerm};
}