using TMPro;
using UnityEngine;

public class CorpGearShopSpeechBubble : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private TextMeshProUGUI speechLabel;
    [SerializeField] private ShopState shopState;
    
    public void Init(PartyCorpAffinity affinity)
    {
        parent.SetActive(false);
    }
}
