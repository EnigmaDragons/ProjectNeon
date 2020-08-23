using TMPro;
using UnityEngine;

public class UIRewardsPresenter : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private TextMeshProUGUI creditsLabel;
    [SerializeField] private CardPresenter cardPrototype;
    [SerializeField] private GameObject cardParent;

    private void OnEnable()
    {
        creditsLabel.text = state.RewardCredits.ToString();
        cardParent.DestroyAllChildren();
        state.RewardCards.ForEach(c => 
            Instantiate(cardPrototype, cardParent.transform)
                .Set(c, () => { }));
    }
}
