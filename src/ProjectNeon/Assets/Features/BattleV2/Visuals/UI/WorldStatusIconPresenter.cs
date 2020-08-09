using TMPro;
using UnityEngine;

public sealed class WorldStatusIconPresenter : StatusIcon
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TextMeshPro label;
    
    public override void Show(Sprite iconImg, string text)
    {
        icon.sprite = iconImg;
        label.text = text;
        gameObject.SetActive(true);
    }
}
