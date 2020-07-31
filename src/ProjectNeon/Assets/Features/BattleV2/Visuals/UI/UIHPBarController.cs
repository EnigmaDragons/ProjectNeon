using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIHPBarController : HPBarControllerBase
{
    [SerializeField] private Image barImage;
    [SerializeField] private TextMeshProUGUI barTextValue;
    [SerializeField] private TextMeshProUGUI shieldTextValue;

    protected override void SetHpFillAmount(float amount)
    {
        barImage.DOFillAmount(amount, 2);
    }

    protected override void SetHpText(string text) => barTextValue.text = text;
    protected override void SetShieldText(string text) => shieldTextValue.text = text;
}
