using TMPro;
using UnityEngine;

[IgnoreForLocalization]
public class PartyBrandAffinityLabel : CorpAffinityUiBase
{
    [SerializeField] private TextMeshProUGUI label;
    
    public override void Init(Corp corp, PartyCorpAffinity affinity)
    {
        label.text = $"Brand Affinity: {affinity[corp]}";
    }
}
