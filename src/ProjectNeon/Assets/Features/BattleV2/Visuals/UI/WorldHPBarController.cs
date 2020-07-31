using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class WorldHPBarController : HPBarControllerBase
{
    [SerializeField] private GameObject bar;
    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private GameObject shieldBar;
    [SerializeField] private TextMeshPro shieldText;

    private float _scaleX;

    void Awake()
    {
        _scaleX = bar.transform.localScale.x;
    }
    
    protected override void SetHpFillAmount(float amount)
    {
        bar.transform.DOScaleX(amount * _scaleX, 2);
    }
    
    protected override void SetShieldFillAmount(float amount)
    {
        shieldBar.transform.DOScaleX(amount * _scaleX, 2);
    }

    protected override void SetHpText(string t) => hpText.text = t;
    protected override void SetShieldText(string t) => shieldText.text = t;
}
