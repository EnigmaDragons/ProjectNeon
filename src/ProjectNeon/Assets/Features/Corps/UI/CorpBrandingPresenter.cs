using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorpBrandingPresenter : CorpUiBase
{
    [SerializeField] private UnityEngine.UI.Extensions.Gradient gradient;
    [SerializeField] private Image logo;
    [SerializeField] private TextMeshProUGUI corpNameLabel;
    
    public override void Init(Corp c)
    {
        if (logo != null)
            logo.sprite = c.Logo;
        gradient.Vertex1 = c.Color1;
        gradient.Vertex2 = c.Color2;
        if (corpNameLabel != null)
            corpNameLabel.text = c.Name;
    }
}