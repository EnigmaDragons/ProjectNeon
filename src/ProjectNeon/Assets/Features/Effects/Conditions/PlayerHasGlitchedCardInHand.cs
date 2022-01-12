using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/PlayerHasGlitchedCardInHand")]
public class PlayerHasGlitchedCardInHand : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.PlayerCardZones.HandZone.Cards.Any(x => x.Mode == CardMode.Glitched)
            ? Maybe<string>.Missing()
            : new Maybe<string>($"No cards in player's hand are glitched");
    }
}