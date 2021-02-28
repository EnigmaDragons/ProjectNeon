using UnityEngine;

[CreateAssetMenu(menuName = "AI/RewardBotAI")]
public class RewardBotAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .WithSelectedUltimateIfAvailable()
            .WithFinalizedCardSelection(CardTag.Healing)
            .WithSelectedTargetsPlayedCard();
    }
}