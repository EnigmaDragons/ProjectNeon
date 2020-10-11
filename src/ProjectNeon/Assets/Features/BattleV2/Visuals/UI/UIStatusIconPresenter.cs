using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class UIStatusIconPresenter : StatusIcon, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;

    private Vector3 _originalIconScale;
    private string _tooltip = "";

    private void Awake()
    {
        _originalIconScale = icon.transform.localScale;
    }
    
    public override void Show(Sprite iconImg, string text, string tooltip)
    {
        icon.sprite = iconImg;
        label.text = text;
        gameObject.SetActive(true);
        _tooltip = tooltip;
        icon.transform.localScale = _originalIconScale;
        gameObject.transform.DOPunchScale(new Vector3(1.28f, 1.28f, 1.28f), 1f, 1);
    }

    public void OnPointerEnter(PointerEventData eventData) => Message.Publish(new ShowTooltip(_tooltip));
    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}
