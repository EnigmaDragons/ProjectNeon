using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class InterpolatedCardDescriptions
{
    private static int RoundUp(float f) => Mathf.RoundToInt(f);
    private static string Bold(this string s) => $"<b>{s}</b>";

    public static string InterpolatedDescription(this Card card) => card.Type.InterpolatedDescription(card.Owner);
    public static string InterpolatedDescription(this CardTypeData card, Maybe<Member> owner)
    {
        var desc = card.Description;

        try
        {
            if (card.Actions == null || card.Actions.Length < 0)
                return desc;
            
            var battleEffects = card.Actions
                .Where(x => x != null)
                .SelectMany(a => a.Actions
                    .Where(c => c.Type == CardBattleActionType.Battle))
                    .Select(b => b.BattleEffect)
                .ToArray();
            
            return InterpolatedDescription(desc, battleEffects, owner);

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
            if (!token.Value.StartsWith("{E"))
                throw new InvalidDataException($"Unable to interpolate for non Battle Effects");
            
            var effectIndex = int.Parse(Regex.Match(token.Result("$1"), "\\[(.*?)\\]").Result("$1"));
            var replacementToken = "{E[" + effectIndex + "]}";
            if (effectIndex >= effects.Length)
                throw new InvalidDataException($"Requested Interpolating {effectIndex}, but only found {effects.Length} Battle Effects");
                
            result = result.Replace(replacementToken, Bold(GenerateEffectDescription(effects[effectIndex], owner)));
        }
            
        return result;
    }
    
    public static string GenerateEffectDescription(EffectData data, Maybe<Member> owner)
    {
        if (data.EffectType == EffectType.Attack)
            return owner.IsPresent
                ? RoundUp(data.FloatAmount * owner.Value.State[StatType.Attack]).ToString()
                : $"{data.FloatAmount}x ATK";
        if (data.EffectType == EffectType.ShieldToughness)
            return owner.IsPresent
                ? RoundUp(data.FloatAmount * owner.Value.State[StatType.Toughness]).ToString()
                : $"{data.FloatAmount}x TGH";
        if (data.EffectType == EffectType.HealMagic)
            return owner.IsPresent
                ? RoundUp(data.FloatAmount * owner.Value.State[StatType.Magic]).ToString()
                : $"{data.FloatAmount}x MAG";
        if (data.EffectType == EffectType.HealFlat)
            return RoundUp(data.FloatAmount).ToString();
        if (data.EffectType == EffectType.SpellFlatDamageEffect)
            return RoundUp(data.FloatAmount).ToString();
        
        Debug.LogWarning($"Description for {data.EffectType} is not implemented.");
        return "%%";
    }
}
