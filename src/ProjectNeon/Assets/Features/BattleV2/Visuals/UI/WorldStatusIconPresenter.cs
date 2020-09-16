using TMPro;
using UnityEngine;

public sealed class WorldStatusIconPresenter : StatusIcon
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TextMeshPro label;
    
    private string _tooltip;

    public override void Show(Sprite iconImg, string text, string tooltip)
    {
        icon.sprite = iconImg;
        label.text = text;
        gameObject.SetActive(true);
        _tooltip = tooltip;
    }
    
    public void OnMouseEnter()
    {
        Debug.Log("Mouse Enter");
        Message.Publish(new ShowTooltip(_tooltip));
    }

    public void OnMouseExit() => Message.Publish(new HideTooltip());
}
