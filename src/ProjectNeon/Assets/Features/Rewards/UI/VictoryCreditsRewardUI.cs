using TMPro;
using UnityEngine;

public class VictoryCreditsRewardUI : OnMessage<ShowCreditsGain>
{
    [SerializeField] private GameObject view;
    [SerializeField] private TextMeshProUGUI creditsLabel;
    [SerializeField] private RarityPresenter rarityPresenter;
    
    protected override void Execute(ShowCreditsGain msg)
    {
        view.SetActive(true);
        creditsLabel.text = msg.NumCredits.ToString();
        rarityPresenter.Set(msg.Rarity);
    }
}
