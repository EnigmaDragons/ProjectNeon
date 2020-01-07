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
        var originalScale = bar.transform.localScale;
        bar.transform.localScale = new Vector3(amount * _scaleX, originalScale.y, originalScale.z);
    }

    protected override void SetText(string text) => this.text.text = text;
}
