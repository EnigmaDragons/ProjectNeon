using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorpBrandingPresenter : CorpUiBase
{
    [SerializeField] private UnityEngine.UI.Extensions.Gradient gradient;
    [SerializeField] private Image logo;
    [SerializeField] private Localize corpNameLabel2;
    
    public override void Init(Corp c)
    {
        if (logo != null)
            logo.sprite = c.Logo;
        gradient.Vertex1 = c.Color1;
        gradient.Vertex2 = c.Color2;
        if (corpNameLabel2 != null)
            corpNameLabel2.SetTerm(c.GetTerm());
    }
}
