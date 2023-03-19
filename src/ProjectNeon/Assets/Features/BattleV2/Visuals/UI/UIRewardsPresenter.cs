using TMPro;
using UnityEngine;

public class UIRewardsPresenter : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI creditsLabel;
    [SerializeField] private CardPresenter cardPrototype;
    [SerializeField] private GameObject cardParent;

    private void OnEnable()
    {
        creditsLabel.text = state.RewardCredits.ToString() + 0;
        cardParent.DestroyAllChildren();
        state.RewardCards.ForEach(c => 
            Instantiate(cardPrototype, cardParent.transform)
                .Set(c, () => { }));
    }
}
