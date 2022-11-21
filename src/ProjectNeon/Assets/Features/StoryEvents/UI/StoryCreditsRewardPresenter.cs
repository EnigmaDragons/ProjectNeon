using I2.Loc;
using TMPro;
using UnityEngine;

public class StoryCreditsRewardPresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private Localize descLabel;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI amountLabel;

    public void Init(int rewardAmount)
    {
        amountLabel.text = rewardAmount.ToString();
        descLabel.SetTerm(rewardAmount > 0 ? "StoryEvents/Gained" : "StoryEvents/Lost");
    }

    public string[] GetLocalizeTerms()
        => new[] { "StoryEvents/Gained", "StoryEvents/Lost" };
}
