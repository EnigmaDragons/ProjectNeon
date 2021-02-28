using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Hacker")]
public class HackerAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedUltimateIfAvailable()
            .IfTruePlayType(x => x.Member.MaxHp() > x.Member.CurrentHp() && x.CardOptions.Any(o => o.Is(CardTag.Escape)), CardTag.Escape)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .WithFinalizedCardSelection(CardTag.StealCredits, CardTag.Attack)
            .WithSelectedTargetsPlayedCard();
    }
}