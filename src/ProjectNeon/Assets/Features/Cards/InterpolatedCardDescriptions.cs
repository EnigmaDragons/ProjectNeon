using System;
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
        if (data.EffectType == EffectType.Attack)
            return owner.IsPresent
                ? RoundUp(data.FloatAmount * owner.Value.State[StatType.Attack]).ToString()
                : $"{data.FloatAmount}x ATK";
        if (data.EffectType == EffectType.DamageSpell 
                || data.EffectType == EffectType.MagicDamageOverTime 
                || data.EffectType == EffectType.HealMagic
                || data.EffectType == EffectType.HealOverTime
                || data.EffectType == EffectType.AdjustStatAdditivelyBaseOnMagicStat)
            return owner.IsPresent
                ? RoundUp(data.FloatAmount * owner.Value.State[StatType.Magic]).ToString()
                : $"{data.FloatAmount}x MAG";
        if (data.EffectType == EffectType.ShieldToughness)
            return owner.IsPresent
                ? RoundUp(data.FloatAmount * owner.Value.State[StatType.Toughness]).ToString()
                : $"{data.FloatAmount}x TGH";
        if (data.EffectType == EffectType.HealFlat)
            return RoundUp(data.FloatAmount).ToString();
        
        Debug.LogWarning($"Description for {data.EffectType} is not implemented.");
        return "%%";
    }
    
    private static string GenerateDurationDescription(EffectData data)
    {
        var value = data.NumberOfTurns.Value;
        var turnString = value < 0
                        ? "the Battle." 
                        : value < 2
                            ? "Current Turn." 
                            : $"{value} Turns.";
        return $"for {turnString}";
    }
}
