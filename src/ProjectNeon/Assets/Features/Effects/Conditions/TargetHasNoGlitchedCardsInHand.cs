using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasNoGlitchedCardsInHand")]
public class TargetHasNoGlitchedCardsInHand : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.PlayerCardZones.HandZone.Cards.Any(x => x.Mode == CardMode.Glitched && x.Owner.Id == ctx.Target.Members[0].Id)
            ? new Maybe<string>($"There is a card of the target's that is glitched")
            : Maybe<string>.Missing();
    }
}