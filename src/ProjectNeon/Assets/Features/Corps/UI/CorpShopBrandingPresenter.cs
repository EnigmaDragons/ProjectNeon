using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorpShopBrandingPresenter : CorpUiBase
{
    [SerializeField] private UnityEngine.UI.Extensions.Gradient gradient;
    [SerializeField] private Image logo;
    [SerializeField] private TextMeshProUGUI corpShopNameLabel;

    public override void Init(Corp c)
    {
        logo.sprite = c.Logo;
        gradient.Vertex1 = c.Color1;
        gradient.Vertex2 = c.Color2;
        corpShopNameLabel.text = c.GearShopData.ShopName;
    }
}
