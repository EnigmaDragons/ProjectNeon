using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasPlayedCardThisTurn")]
public class TargetHasPlayedCardThisTurn : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Target.Members.All(member => ctx.CardsPlayedThisTurn.Any(card => card.Member.Id == member.Id))
            ? Maybe<string>.Missing()
            : new Maybe<string>("One of the targets has not played a card this turn");
    }
}