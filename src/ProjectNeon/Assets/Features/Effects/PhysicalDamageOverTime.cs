using UnityEngine;

public class PhysicalDamageOverTime: Effect
{
    private readonly float _multiplier;
    private readonly int _turns;

    public PhysicalDamageOverTime(EffectData data)
    {
        _multiplier = data.FloatAmount;
        _turns = data.NumberOfTurns;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.ForEach(x => x.State.ApplyTemporaryAdditive(
            new DamageOverTimeState(Mathf.CeilToInt(_multiplier * ctx.Source.Attack()), x, _turns)));
    }
}