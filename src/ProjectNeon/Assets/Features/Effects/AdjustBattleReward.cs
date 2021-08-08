public class AdjustBattleReward : Effect
{
    private readonly string _rewardName;
    private readonly string _amountFormula;

    public AdjustBattleReward(string rewardName, string amountFormula)
    {
        _rewardName = rewardName;
        _amountFormula = amountFormula;
    }
    
    public void Apply(EffectContext ctx)
    {
        var amount = Formula.Evaluate(ctx.SourceStateSnapshot, _amountFormula, ctx.XPaidAmount).CeilingInt();
        ctx.RewardState.Add(_rewardName, amount);
        var sign = amount > 0 ? "+" : "";
        BattleLog.Write($"Battle Reward {_rewardName} adjusted by {sign}{amount}");
    }
}
