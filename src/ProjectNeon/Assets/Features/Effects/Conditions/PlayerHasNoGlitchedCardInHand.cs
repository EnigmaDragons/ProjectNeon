using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/PlayerHasNoGlitchedCardInHand")]
public class PlayerHasNoGlitchedCardInHand : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.PlayerCardZones.HandZone.Cards.Any(x => x.Mode == CardMode.Glitched)
            ? new Maybe<string>($"Player has glitched cards in hand")
            : Maybe<string>.Missing();
    }
}