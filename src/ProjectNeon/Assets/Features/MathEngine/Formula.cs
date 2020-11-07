using System;
using System.Data;
using System.Linq;

public static class Formula
{
    public static float Evaluate(Member src, string expression) 
        => Evaluate(new FormulaContext(src.State, Maybe<MemberState>.Missing()), expression);

    public static float Evaluate(FormulaContext ctx, string expression)
    {
        var newExp = expression;
        
        foreach (var stat in Enum.GetValues(typeof(StatType)).Cast<StatType>())
        {
            if (ctx.Target.IsPresent)
                newExp = newExp.Replace($"Target[{stat}]", ctx.Target.Value[stat].ToString());
            newExp = newExp.Replace(stat.ToString(), ctx.Source[stat].ToString());
        }

        foreach (var stat in Enum.GetValues(typeof(TemporalStatType)).Cast<TemporalStatType>())
        {
            if (ctx.Target.IsPresent)
                newExp = newExp.Replace($"Target[{stat}]", ctx.Target.Value[stat].ToString());
            newExp = newExp.Replace(stat.ToString(), ctx.Source[stat].ToString());
        }

        return Convert.ToSingle(new DataTable().Compute(newExp, null));
    }
}
