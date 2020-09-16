using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public sealed class UIStatusIconPresenter : StatusIcon, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;
    
    private string _tooltip = "";

    public override void Show(Sprite iconImg, string text, string tooltip)
    {
        icon.sprite = iconImg;
        label.text = text;
        gameObject.SetActive(true);
        _tooltip = tooltip;
    }

    public void OnPointerEnter(PointerEventData eventData) => Message.Publish(new ShowTooltip(_tooltip));
    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}
