using TMPro;
using UnityEngine;

public class StoryCreditsRewardPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descLabel;
    [SerializeField] private TextMeshProUGUI amountLabel;

    public void Init(int rewardAmount)
    {
        amountLabel.text = rewardAmount.ToString();
        descLabel.text = rewardAmount > 0 ? "You Gained" : "You Lost";
    }
}
