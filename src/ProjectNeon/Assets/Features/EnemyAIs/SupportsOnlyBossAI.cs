using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/SupportsOnlyBossAI")]
public class SupportsOnlyBossAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return  new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithSelectedTargetsPlayedCard(t => t.Members.All(m => m.BattleRole == BattleRole.Boss));
    }
}
