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
    private static readonly Dictionary<CardTag, string> CardTagSearchTerms = CardTags.ToDictionary(t => t, t => $"Tag[{t}]");
    private static readonly Dictionary<StatType, string> BaseStatSearchTerms = StatTypes.ToDictionary(t => t, t => $"Base[{t}]");
    
    private static int RoundUp(float f) => f > 0 ? Mathf.CeilToInt(f) : Mathf.FloorToInt(f);

    public static int EvaluateToIntWithDoubleDamage(FormulaContext ctx, string expression)
    {
        var result = EvaluateToInt(ctx, expression);
        return ctx.Source.Stats[TemporalStatType.DoubleDamage] > 0 ? result * 2 : result;
    }

    public static int EvaluateToInt(MemberStateSnapshot src, string expression, ResourceQuantity xAmountPaid, EffectScopedData scopedData) 
        => EvaluateToInt(new FormulaContext(src, Maybe<MemberState>.Missing(), xAmountPaid, scopedData), expression);
    
    public static int EvaluateToInt(MemberStateSnapshot snapshot, MemberState target, string expression, ResourceQuantity xAmountPaid, EffectScopedData scopedData) 
        => EvaluateToInt(new FormulaContext(snapshot, target, xAmountPaid, scopedData), expression);

    public static int EvaluateToInt(FormulaContext ctx, string expression) => RoundUp(Evaluate(ctx, expression));

    public static float EvaluateRaw(FormulaContext ctx, string expression) => Evaluate(ctx, expression);

    public static float EvaluateRaw(MemberStateSnapshot snapshot, MemberState target, string expression, ResourceQuantity xAmountPaid, EffectScopedData scopedData) 
        => Evaluate(new FormulaContext(snapshot, target, xAmountPaid, scopedData), expression);
    
    private static float Evaluate(FormulaContext ctx, string expression)
    {
        var newExp = string.IsNullOrWhiteSpace(expression) ? "0" : expression;
        newExp = newExp.Replace("PrimaryStat", "Power");
        newExp = ReplaceNamedVariables(newExp, ctx);
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
        foreach (var tag in CardTagSearchTerms)
            expression = ReplaceIfContains(expression, tag.Value, () => ctx.Source.TagsPlayed[tag.Key].GetString());
        return expression;
    }

    private static string ReplaceIfContains(string expression, string ifContains, Func<string> getReplacementValue)
    {
        if (expression.ContainsAnyCase(ifContains))
            expression = expression.Replace(ifContains, getReplacementValue());
        return expression;
    }

    private static string ReplaceShorthandStatNames(string expression)
    {
        foreach (var stat in StatTypeAliases.AbbreviationToFullNames) 
            expression = ReplaceIfContains(expression, stat.Key, () => stat.Value);
        return expression;
    }

    private static readonly string TargetMaxPrimaryResource = "Target[MaxPrimaryResource]";
    private static readonly string TargetPrimaryResource = "Target[PrimaryResource]";
    private static readonly string MaxPrimaryResource = "MaxPrimaryResource";
    private static readonly string PrimaryResource = "PrimaryResource";
    private static string ReplaceResources(string expression, FormulaContext ctx)
    {
        if (ctx.Target.IsPresent && expression.ContainsAnyCase(TargetString))
        {
            expression = ReplaceIfContains(expression, TargetMaxPrimaryResource, () => ctx.Target.Value.PrimaryResource.MaxAmount.GetString());
            expression = ReplaceIfContains(expression, TargetPrimaryResource, () => ctx.Target.Value.PrimaryResourceAmount.GetString());
        }
        expression = expression.Replace(MaxPrimaryResource, ctx.Source.PrimaryResource.MaxAmount.GetString());
        expression = expression.Replace(PrimaryResource, ctx.Source.PrimaryResourceAmount.GetString());
        foreach (var resourceType in ctx.Source.ResourceTypes)
        {
            expression = expression.Replace(resourceType.Name, ctx.Source[resourceType].GetString());
        }
        return expression;
    }

    private static string ReplaceStats(string expression, FormulaContext ctx)
    {
        foreach (var stat in StatTypes)
        {
            expression = ReplaceOrRemoveTargetValue(expression, stat, ctx);
            expression = expression.Replace(BaseStatSearchTerms[stat], ctx.Source.BaseStats[stat].CeilingInt().GetString());
            expression = expression.Replace(stat.GetString(), ctx.Source[stat].CeilingInt().GetString());
        }
        return expression;
    }
    
    private static string ReplaceTemporalStats(string expression, FormulaContext ctx)
    {
        foreach (var stat in TemporalStatTypes)
        {
            expression = ReplaceOrRemoveTargetValue(expression, stat, ctx);
            expression = expression.Replace(stat.GetString(), ctx.Source[stat].GetString());
        }
        return expression;
    }

    private const string TargetString = "Target";
    private const string TargetBaseString = "TargetBase";
    private static string ReplaceOrRemoveTargetValue(string expression, StatType stat, FormulaContext ctx)
    {        
        if (!expression.ContainsAnyCase(TargetString))
            return expression;
        
        ctx.Target.ExecuteIfPresentOrElse(
            t => expression = ReplaceIfContains(expression, $"Target[{stat}]", () => t[stat].CeilingInt().GetString()), 
            () => expression = ReplaceIfContains(expression, $"Target[{stat}]", () => string.Empty));
        
        if (!expression.ContainsAnyCase(TargetBaseString))
            return expression;
        
        ctx.Target.ExecuteIfPresentOrElse(
            t => expression = expression.Replace($"TargetBase[{stat}]", t.BaseStats[stat].CeilingInt().GetString()), 
            () => expression = expression.Replace($"TargetBase[{stat}]", ""));
        return expression;
    }
    
    private static string ReplaceOrRemoveTargetValue(string expression, TemporalStatType stat, FormulaContext ctx)
    {
        if (!expression.ContainsAnyCase(TargetString))
            return expression;
        
        ctx.Target.ExecuteIfPresentOrElse(
            t => expression = expression.Replace($"Target[{stat}]", t[stat].CeilingInt().GetString()), 
            () => expression = expression.Replace($"Target[{stat}]", ""));
        
        if (!expression.ContainsAnyCase(TargetBaseString))
            return expression;
        
        ctx.Target.ExecuteIfPresentOrElse(
            t => expression = expression.Replace($"TargetBase[{stat}]", t.BaseStats[stat].CeilingInt().GetString()), 
            () => expression = expression.Replace($"TargetBase[{stat}]", ""));
        return expression;
    }

    private const string XString = "X";
    private static string ReplaceXCost(string expression, FormulaContext ctx)
        => expression.Replace(XString, ctx.XAmountPaid.Amount.GetString());

    private static Regex NamedVariableRegex = new Regex(@"Variable\[(.+?)]");
    private static string ReplaceNamedVariables(string expression, FormulaContext ctx)
        => NamedVariableRegex.Replace(expression, x => ctx.ScopedData.GetVariable(x.Groups[1].Value).ToString());
    
    private const string ConditionalsString = "{(.*):(.*)}";
    private static string ResolveConditionals(string expression, DataTable dataTable) 
        => Regex.Replace(expression, ConditionalsString, x => ResolveCondition(dataTable, x));

    private static string ResolveCondition(DataTable dataTable, Match match)
        => (bool)dataTable.Compute(match.Groups[1].Value, null)
            ? match.Groups[2].Value
            : string.Empty;

    private static string Wrapper(string specifier, string wrapped) => $"{specifier}[{wrapped}]";
}
