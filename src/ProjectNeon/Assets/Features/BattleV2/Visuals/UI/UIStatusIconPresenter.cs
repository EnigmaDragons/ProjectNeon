using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIStatusIconPresenter : StatusIcon
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;

    public override void Show(Sprite iconImg, string text)
    {
        icon.sprite = iconImg;
        label.text = text;
        gameObject.SetActive(true);
    }
}
