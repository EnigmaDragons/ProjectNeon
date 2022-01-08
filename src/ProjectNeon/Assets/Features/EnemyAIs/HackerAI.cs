using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Hacker")]
public class HackerAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .WithFinalizedCardSelection(CardTag.GlitchHand, CardTag.Attack)
            .WithSelectedTargetsPlayedCard();
    }
}