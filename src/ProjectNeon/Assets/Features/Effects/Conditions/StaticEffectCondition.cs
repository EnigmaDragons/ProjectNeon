using UnityEngine;

public abstract class StaticEffectCondition : ScriptableObject, EffectCondition
{
    public abstract Maybe<string> GetShouldNotApplyReason(EffectContext ctx);
}