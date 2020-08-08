using TMPro;
using UnityEngine;

public class UIRewardsPresenter : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private TextMeshProUGUI creditsLabel;

    private void OnEnable()
    {
        creditsLabel.text = state.RewardCredits.ToString();
    }
}
