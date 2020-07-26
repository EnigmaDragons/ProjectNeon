﻿using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIHPBarController : HPBarControllerBase
{
    [SerializeField] private Image barImage;
    [SerializeField] private TextMeshProUGUI barTextValue;

    protected override void SetFillAmount(float amount)
    {
        barImage.DOFillAmount(amount, 2);
    }

    protected override void SetText(string text) => barTextValue.text = text;
}
