using TMPro;
using UnityEngine;

public class CorpGearShopSpeechBubble : CorpAffinityUiBase
{
    [SerializeField] private GameObject parent;
    [SerializeField] private DefaultCorpAffinityLines defaultLines;
    [SerializeField] private TextMeshProUGUI speechLabel;
    
    public override void Init(Corp corp, PartyCorpAffinity affinity)
    {
        parent.SetActive(false);
        var corpLine = corp.GearShopData.AffinityLines.RandomLine(affinity[corp]);
        if (corpLine.IsPresent && !string.IsNullOrWhiteSpace(corpLine.Value))
        {
            parent.SetActive(true);
            speechLabel.text = corpLine.Value;
        }
        else
        {
            var defaultLine = defaultLines.Lines.RandomLine(affinity[corp]);
            if (defaultLine.IsPresent)
            {
                parent.SetActive(true);
                speechLabel.text = defaultLine.Value;
            }
        }
    }
}
