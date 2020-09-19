using UnityEngine;

public class MagicDamageOverTime : Effect
{
    private readonly float _multiplier;
    private readonly int _turns;

    public MagicDamageOverTime(EffectData data)
    {
        _multiplier = data.FloatAmount;
        _turns = data.NumberOfTurns;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.ForEach(x => x.State.ApplyTemporaryAdditive(
            new DamageOverTimeState(Mathf.CeilToInt(_multiplier * ctx.Source.Magic()), x, _turns)));
    }
}
