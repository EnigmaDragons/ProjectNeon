using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gradient = UnityEngine.UI.Extensions.Gradient;

public class CorpLightDetailPanelWithRival : CorpUiBase
{
    [SerializeField] private AllCorps corps;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Gradient corpGradient;
    [SerializeField] private TextMeshProUGUI shortDescriptionLabel;
    [SerializeField] private GameObject rivalParent;
    [SerializeField] private Image rivalLogo;
    
    public override void Init(Corp c)
    {
        nameLabel.text = c.Name;
        corpGradient.Vertex1 = c.Color1;
        corpGradient.Vertex2 = c.Color2;
        shortDescriptionLabel.text = c.ShortDescription;

        var rival = c.RivalCorpNames.FirstAsMaybe().Map(r => corps.GetCorpByName(r));
        rivalParent.SetActive(true);
        if (rival.IsPresent && rival.Value.IsPresent)
            rivalLogo.sprite = rival.Value.Value.Logo;
        else
            rivalParent.SetActive(false);
    }
}