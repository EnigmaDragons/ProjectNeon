using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class InterpolatedCardDescriptions
{
    private static int RoundUp(float f) => Mathf.CeilToInt(f);
    private static string Bold(this string s) => $"<b>{s}</b>";

    public static string InterpolatedDescription(this Card card) => card.Type.InterpolatedDescription(card.Owner);
    public static string InterpolatedDescription(this CardTypeData card, Maybe<Member> owner)
    {
        var desc = card.Description;

        try
        {
            if (card.Actions() == null || card.Actions().Length < 0)
                return desc;

            var battleEffects = card.Actions()
                .Where(x => x != null)
                .SelectMany(a => a.Actions.Where(c => c.Type == CardBattleActionType.Battle))
                .Select(b => b.BattleEffect);

            var conditionalBattleEffects = card.Actions()
                .Where(x => x != null)
                .SelectMany(a => a.Actions.Where(c => c.Type == CardBattleActionType.Condition))
                .SelectMany(b => b.ConditionData.ReferencedEffect.Actions.Select(a => a.BattleEffect));
            
            return InterpolatedDescription(desc, battleEffects.Concat(conditionalBattleEffects).ToArray(), owner);

        }
        catch (Exception e)
        {
            Log.Error($"Unable to Generate Interpolated Description for {card.Name}");
            Debug.LogException(e);
            return desc;
        }
    }

    public static string InterpolatedDescription(string desc, EffectData[] effects, Maybe<Member> owner)
    {
        var result = desc;
        
        var tokens = Regex.Matches(desc, "{(.*?)}");
        foreach (Match token in tokens)
        {
            if (!token.Value.StartsWith("{E") && !token.Value.StartsWith("{D"))
                throw new InvalidDataException($"Unable to interpolate for things other than Battle Effects and Durations");

            var effectIndex = int.Parse(Regex.Match(token.Result("$1"), "\\[(.*?)\\]").Result("$1"));
            if (effectIndex >= effects.Length)
                throw new InvalidDataException($"Requested Interpolating {effectIndex}, but only found {effects.Length} Battle Effects");

            var effectReplacementToken = "{E[" + effectIndex + "]}";
            if (result.Contains("{E["))
                result = result.Replace(effectReplacementToken, Bold(GenerateEffectDescription(effects[effectIndex], owner)));

            var durationReplacementToken = "{D[" + effectIndex + "]}";
            result = result.Replace(durationReplacementToken, GenerateDurationDescription(effects[effectIndex]));
        }
        return result;
    }
    
    public static string GenerateEffectDescription(EffectData data, Maybe<Member> owner)
    {
        if (data.EffectType == EffectType.Attack
            || data.EffectType == EffectType.PhysicalDamageOverTime)
            return owner.IsPresent
                ? RoundUp(data.FloatAmount * owner.Value.State[StatType.Attack]).ToString()
                : $"{data.FloatAmount}x ATK";
        if (data.EffectType == EffectType.AdjustStatAdditivelyFormula)
            return owner.IsPresent
                ? RoundUp(Formula.Evaluate(owner.Value, data.Formula)).ToString()
                : FormattedFormula(data.Formula);
        if (data.EffectType == EffectType.DamageSpell 
                || data.EffectType == EffectType.MagicDamageOverTime 
                || data.EffectType == EffectType.HealMagic
                || data.EffectType == EffectType.HealOverTime
                || data.EffectType == EffectType.AdjustStatAdditivelyBaseOnMagicStat)
            return owner.IsPresent
                ? RoundUp(data.BaseAmount + data.FloatAmount * owner.Value.State[StatType.Magic]).ToString()
                : WithBaseAmount(data, "x MAG");
        if (data.EffectType == EffectType.ShieldToughness
            || data.EffectType == EffectType.HealToughness)
            return owner.IsPresent
                ? RoundUp(data.FloatAmount * owner.Value.State[StatType.Toughness]).ToString()
                : $"{data.FloatAmount}x TGH";
        if (data.EffectType == EffectType.AdjustCounter)
            return $"{data.EffectScope} {data.IntAmount + data.BaseAmount}";
        
        if (data.EffectType == EffectType.ShieldToughnessBasedOnNumberOfOpponentDoTs)
            return owner.IsPresent
                ? RoundUp(Mathf.Min(owner.Value.MaxShield(),(data.FloatAmount * owner.Value.State[StatType.Toughness]))).ToString()
                : $"{data.FloatAmount}x TGH";
        
        Debug.LogWarning($"Description for {data.EffectType} is not implemented.");
        return "%%";
    }

    private static string WithBaseAmount(EffectData data, string floatString)
    {
        var baseAmount = data.BaseAmount != 0 ? $"{data.BaseAmount.Value}" : "";
        var floatAmount = data.FloatAmount > 0 ? $"{data.FloatAmount.Value}{floatString}" : "";
        return baseAmount + floatAmount;
    }

    private static string GenerateDurationDescription(EffectData data)
    {
        var value = data.NumberOfTurns.Value;
        var turnString = value < 0
                        ? "the Battle." 
                        : value < 2
                            ? "Current Turn." 
                            : $"{Bold(value.ToString())} Turns.";
        return $"for {turnString}";
    }

    private static string FormattedFormula(string s)
    {
        var newS = s;
        newS = newS.Replace(" * ", "x ");
        foreach (var stat in StatAbbreviations)
        {
            if (newS.Contains(stat.Key))
                newS = newS.Replace(stat.Key, stat.Value);
        }

        return newS;
    }
    
    private static Dictionary<string, string> StatAbbreviations = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
        { StatType.Leadership.ToString(), "LEAD" },
        { StatType.Attack.ToString(), "ATK" },
        { StatType.Magic.ToString(), "MAG" },
        { StatType.Armor.ToString(), "ARM" },
        { StatType.Toughness.ToString(), "TGH" },
    }; 
}
