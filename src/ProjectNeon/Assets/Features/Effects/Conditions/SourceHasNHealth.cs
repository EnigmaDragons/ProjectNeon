using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasNHealth")]
public class SourceHasNHealth : StaticEffectCondition
{
    [SerializeField] private float minHealthFactor = 1f;
    
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.CurrentHp() >= Mathf.FloorToInt(ctx.Source.MaxHp() * minHealthFactor)
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.NameTerm.ToEnglish()} did not have enough health.");
    }
}
