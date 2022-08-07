using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Formula
{
    private static readonly StatType[] StatTypes = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToArray();
    private static readonly TemporalStatType[] TemporalStatTypes = Enum.GetValues(typeof(TemporalStatType)).Cast<TemporalStatType>().ToArray();
    private static readonly CardTag[] CardTags = Enum.GetValues(typeof(CardTag)).Cast<CardTag>().ToArray();
    
    private static int RoundUp(float f) => f > 0 ? Mathf.CeilToInt(f) : Mathf.FloorToInt(f);
    
    public static int EvaluateToInt(MemberStateSnapshot src, string expression, ResourceQuantity xAmountPaid) 
        => EvaluateToInt(new FormulaContext(src, Maybe<MemberState>.Missing(), xAmountPaid), expression);
    
    public static int EvaluateToInt(MemberStateSnapshot snapshot, MemberState target, string expression, ResourceQuantity xAmountPaid) 
        => EvaluateToInt(new FormulaContext(snapshot, target, xAmountPaid), expression);

    public static int EvaluateToInt(FormulaContext ctx, string expression) => RoundUp(Evaluate(ctx, expression));

    public static float EvaluateRaw(FormulaContext ctx, string expression) => Evaluate(ctx, expression);
    
    public static float EvaluateRaw(MemberStateSnapshot src, string expression, ResourceQuantity xAmountPaid) 
        => Evaluate(new FormulaContext(src, Maybe<MemberState>.Missing(), xAmountPaid), expression);
    
    public static float EvaluateRaw(MemberStateSnapshot snapshot, MemberState target, string expression, ResourceQuantity xAmountPaid) 
        => Evaluate(new FormulaContext(snapshot, target, xAmountPaid), expression);
    
    private static float Evaluate(FormulaContext ctx, string expression)
    {
        var newExp = string.IsNullOrWhiteSpace(expression) ? "0" : expression;
        newExp = newExp.Replace("PrimaryStat", "Power");
        newExp = ReplaceTags(newExp, ctx);
        newExp = ReplaceShorthandStatNames(newExp);
        newExp = ReplaceResources(newExp, ctx);
        newExp = ReplaceStats(newExp, ctx);
        newExp = ReplaceTemporalStats(newExp, ctx);
        newExp = ReplaceXCost(newExp, ctx);
        try
        {
            var dataTable = new DataTable();
            newExp = ResolveConditionals(newExp, dataTable);
            var result = Convert.ToSingle(dataTable.Compute(newExp, null));
            return result;
        }
        catch (Exception e)
        {
            Log.Error($"Unable to resolve formula. Original: '{expression}' Interpolated: '{newExp}'");
            #if UNITY_EDITOR
            throw;
            #endif
            return 0;
        }
    }
    
    private static string ReplaceTags(string expression, FormulaContext ctx)
    {
        foreach (var tag in CardTags)
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
        if (ctx.Target.IsPresent && expression.ContainsAnyCase("Target"))
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
        foreach (var stat in StatTypes)
        {
            expression = ReplaceOrRemoveTargetValue(expression, stat, ctx);
            expression = expression.Replace($"Base[{stat}]", ctx.Source.BaseStats[stat].CeilingInt().ToString());
            expression = expression.Replace(stat.ToString(), ctx.Source[stat].CeilingInt().ToString());
        }
        return expression;
    }
    
    private static string ReplaceTemporalStats(string expression, FormulaContext ctx)
    {
        foreach (var stat in TemporalStatTypes)
        {
            expression = ReplaceOrRemoveTargetValue(expression, stat, ctx);
            expression = expression.Replace(stat.ToString(), ctx.Source[stat].ToString());
        }
        return expression;
    }
    
    private static string ReplaceOrRemoveTargetValue(string expression, StatType stat, FormulaContext ctx)
    {        
        if (!expression.ContainsAnyCase("Target"))
            return expression;
        
        ctx.Target.ExecuteIfPresentOrElse(
            t => expression = expression.Replace($"Target[{stat}]", t[stat].CeilingInt().ToString()), 
            () => expression = expression.Replace($"Target[{stat}]", ""));
        ctx.Target.ExecuteIfPresentOrElse(
            t => expression = expression.Replace($"TargetBase[{stat}]", t.BaseStats[stat].CeilingInt().ToString()), 
            () => expression = expression.Replace($"TargetBase[{stat}]", ""));
        return expression;
    }
    
    private static string ReplaceOrRemoveTargetValue(string expression, TemporalStatType stat, FormulaContext ctx)
    {
        if (!expression.ContainsAnyCase("Target"))
            return expression;
        
        ctx.Target.ExecuteIfPresentOrElse(
            t => expression = expression.Replace($"Target[{stat}]", t[stat].CeilingInt().ToString()), 
            () => expression = expression.Replace($"Target[{stat}]", ""));
        ctx.Target.ExecuteIfPresentOrElse(
            t => expression = expression.Replace($"TargetBase[{stat}]", t.BaseStats[stat].CeilingInt().ToString()), 
            () => expression = expression.Replace($"TargetBase[{stat}]", ""));
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
