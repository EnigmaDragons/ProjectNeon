using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorpClinicBrandingPresenter : CorpUiBase
{
    [SerializeField] private UnityEngine.UI.Extensions.Gradient gradient;
    [SerializeField] private Image logo;
    [SerializeField] private TextMeshProUGUI nameLabel;
    
    public override void Init(Corp c)
    {
        logo.sprite = c.Logo;
        gradient.Vertex1 = c.Color1;
        gradient.Vertex2 = c.Color2;
        nameLabel.text = c.ClinicName;
    }
}
