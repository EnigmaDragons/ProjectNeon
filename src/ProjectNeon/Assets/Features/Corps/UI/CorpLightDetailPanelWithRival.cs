using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gradient = UnityEngine.UI.Extensions.Gradient;

public class CorpLightDetailPanelWithRival : CorpUiBase, ILocalizeTerms
{
    [SerializeField] private AllCorps corps;
    [SerializeField] private Localize nameLabel;
    [SerializeField] private Gradient corpGradient;
    [SerializeField] private Localize shortDescriptionLabel;
    [SerializeField] private GameObject rivalParent;
    [SerializeField] private Image rivalLogo;
    [SerializeField] private Localize rivalTextLabel;

    public override void Init(Corp c)
    {
        nameLabel.SetTerm(c.GetTerm());
        corpGradient.Vertex1 = c.Color1;
        corpGradient.Vertex2 = c.Color2;
        shortDescriptionLabel.SetTerm(c.ShortDescriptionTerm);

        var rival = c.RivalCorpNames.FirstAsMaybe().Map(r => corps.GetCorpByName(r));
        if (rival.IsPresent && rival.Value.IsPresent)
        {
            rivalLogo.sprite = rival.Value.Value.Logo;
            rivalLogo.color = Color.white;
            rivalTextLabel.SetTerm("MegaCorps/Rival");
        }
        else
        {
            rivalLogo.sprite = null;
            rivalLogo.color = new Color(0, 0, 0, 0);
            rivalTextLabel.SetTerm("MegaCorps/No Rival");
        }
    }

    public string[] GetLocalizeTerms()
        => new[] {"MegaCorps/Rival", "MegaCorps/No Rival"};
}