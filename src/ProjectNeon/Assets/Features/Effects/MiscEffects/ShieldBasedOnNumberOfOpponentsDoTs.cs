using System.Linq;

public class ShieldBasedOnNumberOfOpponentsDoTs : Effect
{
    private readonly float _factor;

    public ShieldBasedOnNumberOfOpponentsDoTs(float factor)
    {
        _factor = factor;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConsciousMembers(m => m.State.AdjustShield(
            _factor 
                * m.MaxShield()
                * ctx.BattleMembers.Values
                    .ToArray()
                    .GetConsciousEnemies(m)
                    .Sum(opp => opp.State.StatusesOfType(StatusTag.DamageOverTime).Length)));
    }
}
