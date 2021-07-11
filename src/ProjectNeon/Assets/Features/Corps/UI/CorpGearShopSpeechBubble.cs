using TMPro;
using UnityEngine;

public class CorpGearShopSpeechBubble : CorpAffinityUiBase
{
    [SerializeField] private GameObject parent;
    [SerializeField] private TextMeshProUGUI speechLabel;
    [SerializeField] private ShopState shopState;
    
    public override void Init(Corp corp, PartyCorpAffinity affinity)
    {
        parent.SetActive(false);
    }
}
