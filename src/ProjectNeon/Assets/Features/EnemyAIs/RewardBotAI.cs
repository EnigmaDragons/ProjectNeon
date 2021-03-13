using UnityEngine;

[CreateAssetMenu(menuName = "AI/RewardBotAI")]
public class RewardBotAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .WithSelectedUltimateIfAvailable()
            .WithFinalizedCardSelection(CardTag.Healing)
            .WithSelectedTargetsPlayedCard();
    }
}