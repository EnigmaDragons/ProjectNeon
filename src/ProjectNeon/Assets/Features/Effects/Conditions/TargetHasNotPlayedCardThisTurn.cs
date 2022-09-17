using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasNotPlayedCardThisTurn")]
public class TargetHasNotPlayedCardThisTurn : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Target.Members.Any(member => ctx.CardsPlayedThisTurn.Any(card => card.Member.Id == member.Id))
            ? new Maybe<string>("One of the targets has played a card this turn")
            : Maybe<string>.Missing();
    }
}