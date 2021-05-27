using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/NeverApplyThis")]
public class NeverApplyThisEffectCondition : StaticEffectCondition
{
    // This class is a clever way to get interpolated number
    // pieces for complex formulas without impacting the actual
    // effect of a card
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx) => new Maybe<string>("Never Apply");
}