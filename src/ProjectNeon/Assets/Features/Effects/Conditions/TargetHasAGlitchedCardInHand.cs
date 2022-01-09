using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasAGlitchedCardInHand")]
public class TargetHasAGlitchedCardInHand : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.PlayerCardZones.HandZone.Cards.Any(x => x.Mode == CardMode.Glitched && x.Owner.Id == ctx.Target.Members[0].Id)
            ? Maybe<string>.Missing()
            : new Maybe<string>($"No cards in player's hand are glitched");
    }
}