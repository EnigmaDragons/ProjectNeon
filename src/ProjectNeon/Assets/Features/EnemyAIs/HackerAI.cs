using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Hacker")]
public class HackerAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedUltimateIfAvailable()
            .IfTruePlayType(x => x.Member.MaxHp() > x.Member.CurrentHp() && x.CardOptions.Any(o => o.Is(CardTag.Escape)), CardTag.Escape)
            .IfTrueDontPlayType(x => x.Member.MaxHp() <= x.Member.CurrentHp(), CardTag.Escape)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .WithFinalizedCardSelection(CardTag.StealCredits, CardTag.Attack)
            .WithSelectedTargetsPlayedCard();
    }
}