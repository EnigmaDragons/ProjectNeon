using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class WorldHPBarController : HPBarControllerBase
{
    [SerializeField] private GameObject bar;
    [SerializeField] private TextMeshPro text;

    private float _scaleX;

    void Awake()
    {
        _scaleX = bar.transform.localScale.x;
    }
    
    protected override void SetFillAmount(float amount)
    {
        bar.transform.DOScaleX(amount * _scaleX, 2);
    }

    protected override void SetText(string text) => this.text.text = text;
}
