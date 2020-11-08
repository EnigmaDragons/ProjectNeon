using System.Linq;

public class ShieldToughtnessBasedOnNumberOfOpponentDoTs : Effect
{
    private readonly float _factor;

    public ShieldToughtnessBasedOnNumberOfOpponentDoTs(float factor)
    {
        _factor = factor;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConsciousMembers(m => m.State.AdjustShield(
            _factor 
            * m.Toughness()
            * ctx.BattleMembers.Values
              .ToArray()
              .GetConsciousEnemies(m)
              .Sum(opp => opp.State.StatusesOfType(StatusTag.DamageOverTime).Length)));
    }
}
