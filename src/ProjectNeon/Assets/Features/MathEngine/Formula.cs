using System;
using System.Data;
using System.Linq;

public static class Formula
{
    public static float Evaluate(Member src, string expression) 
        => Evaluate(new FormulaContext(src), expression);

    public static float Evaluate(FormulaContext ctx, string expression)
    {
        var newExp = expression;
        
        foreach (var stat in Enum.GetValues(typeof(StatType)).Cast<StatType>())
            newExp = newExp.Replace(stat.ToString(), ctx.Source.State[stat].ToString());
        
        foreach (var stat in Enum.GetValues(typeof(TemporalStatType)).Cast<TemporalStatType>())
            newExp = newExp.Replace(stat.ToString(), ctx.Source.State[stat].ToString());

        return Convert.ToSingle(new DataTable().Compute(newExp, null));
    }
}
