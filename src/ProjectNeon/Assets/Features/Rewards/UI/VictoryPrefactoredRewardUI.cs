using TMPro;
using UnityEngine;

public class VictoryPrefactoredRewardUI : OnMessage<ShowPrefactoredReward>
{
    [SerializeField] private GameObject view;
    [SerializeField] private TextMeshProUGUI creditsLabel;
    [SerializeField] private TextMeshProUGUI clinicVouchersLabel;
    
    protected override void Execute(ShowPrefactoredReward msg)
    {
        view.SetActive(true);
        creditsLabel.text = msg.NumCredits.ToString();
        clinicVouchersLabel.text = msg.NumClinicVouchers.ToString();
    }
}
