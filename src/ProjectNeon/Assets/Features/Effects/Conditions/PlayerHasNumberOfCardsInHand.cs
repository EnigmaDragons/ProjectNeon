using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/PlayerHasNumberOfCardsInHand")]
public class PlayerHasNumberOfCardsInHand : StaticEffectCondition
{
    [SerializeField] private int number;
    [SerializeField] private bool inclusiveBound = true;
    [SerializeField] private bool greater = true;
    
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var cardsInHands = ctx.PlayerCardZones.HandZone.Count;
        if (inclusiveBound && cardsInHands == number)
            return Maybe<string>.Missing();
        if (greater && cardsInHands > number)
            return Maybe<string>.Missing();
        if (!greater && cardsInHands < number)
            return Maybe<string>.Missing();
        var comparisonWord = greater ? "more" : "fewer";
        var inclusiveWord = inclusiveBound ? " or equal to" : " than";
        return Maybe<string>.Present($"Did not have {comparisonWord}{inclusiveWord} {number} Cards in Hand");
    }
}
