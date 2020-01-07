using TMPro;
using UnityEngine;

public sealed class WorldHPBarController : HPBarControllerBase
{
    [SerializeField] private GameObject bar;
    [SerializeField] private TextMeshPro text;

    protected override void SetFillAmount(float amount)
    {
        var originalScale = bar.transform.localScale;
        bar.transform.localScale = new Vector3(amount, originalScale.y, originalScale.z);
    }

    protected override void SetText(string text) => this.text.text = text;
}
