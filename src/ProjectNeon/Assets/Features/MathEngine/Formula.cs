using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

public static class Formula
{
    public static float Evaluate(MemberStateSnapshot src, string expression, ResourceQuantity xAmountPaid) 
        => Evaluate(new FormulaContext(src, Maybe<MemberState>.Missing(), xAmountPaid), expression);
    
    public static float Evaluate(MemberStateSnapshot snapshot, MemberState target, string expression, ResourceQuantity xAmountPaid) 
        => Evaluate(new FormulaContext(snapshot, target, xAmountPaid), expression);
    
    public static float Evaluate(FormulaContext ctx, string expression)
    {
        var newExp = string.IsNullOrWhiteSpace(expression) ? "0" : expression;
        newExp = ReplaceTags(newExp, ctx);
        newExp = ReplaceShorthandStatNames(newExp);
        newExp = ReplaceResources(newExp, ctx);
        newExp = ReplaceStats(newExp, ctx);
        newExp = ReplaceTemporalStats(newExp, ctx);
        newExp = ReplaceXCost(newExp, ctx);
        var dataTable = new DataTable();
        newExp = ResolveConditionals(newExp, dataTable);
        return Convert.ToSingle(dataTable.Compute(newExp, null));
    }
    
    private static string ReplaceTags(string expression, FormulaContext ctx)
    {
        foreach (var tag in Enum.GetValues(typeof(CardTag)).Cast<CardTag>())
            expression = expression.Replace($"Tag[{tag}]", ctx.Source.TagsPlayed[tag].ToString());
        return expression;
    }

    private static string ReplaceShorthandStatNames(string expression)
    {
        foreach (var stat in StatTypeAliases.AbbreviationToFullNames) 
            expression = expression.Replace(stat.Key, stat.Value);
        return expression;
    }

    private static string ReplaceResources(string expression, FormulaContext ctx)
    {
        if (ctx.Target.IsPresent)
        {
            expression = expression.Replace("Target[MaxPrimaryResource]", ctx.Target.Value.PrimaryResource.MaxAmount.ToString());
            expression = expression.Replace("Target[PrimaryResource]", ctx.Target.Value.PrimaryResourceAmount.ToString());
        }
        expression = expression.Replace("MaxPrimaryResource", ctx.Source.PrimaryResource.MaxAmount.ToString());
        expression = expression.Replace("PrimaryResource", ctx.Source.PrimaryResourceAmount.ToString());
        foreach (var resourceType in ctx.Source.ResourceTypes)
        {
            expression = expression.Replace(resourceType.Name, ctx.Source[resourceType].ToString());
        }
        return expression;
    }

    private static string ReplaceStats(string expression, FormulaContext ctx)
    {
        foreach (var stat in Enum.GetValues(typeof(StatType)).Cast<StatType>())
        {
            if (ctx.Target.IsPresent)
                expression = expression.Replace($"Target[{stat}]", ctx.Target.Value[stat].ToString());
            expression = expression.Replace(stat.ToString(), ctx.Source[stat].ToString());
        }
        return expression;
    }

    private static string ReplaceTemporalStats(string expression, FormulaContext ctx)
    {
        foreach (var stat in Enum.GetValues(typeof(TemporalStatType)).Cast<TemporalStatType>())
        {
            if (ctx.Target.IsPresent)
                expression = expression.Replace($"Target[{stat}]", ctx.Target.Value[stat].ToString());
            expression = expression.Replace(stat.ToString(), ctx.Source[stat].ToString());
        }
        return expression;
    }
    
    private static string ReplaceXCost(string expression, FormulaContext ctx)
        => expression.Replace("X", ctx.XAmountPaid.Amount.ToString());
    
    private static string ResolveConditionals(string expression, DataTable dataTable) 
        => Regex.Replace(expression, "{(.*):(.*)}", x => ResolveCondition(dataTable, x));

    private static string ResolveCondition(DataTable dataTable, Match match)
        => (bool)dataTable.Compute(match.Groups[1].Value, null)
            ? match.Groups[2].Value
            : "";
}
