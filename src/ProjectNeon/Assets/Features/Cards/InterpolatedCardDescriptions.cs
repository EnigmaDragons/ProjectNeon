using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class InterpolatedCardDescriptions
{
    private static int RoundUp(float f) => Mathf.CeilToInt(f);
    private static string Bold(this string s) => $"<b>{s}</b>";

    public static string InterpolatedDescription(this Card card, ResourceQuantity xCost) 
        => card.Type.InterpolatedDescription(card.Owner, xCost);
    public static string InterpolatedDescription(this CardTypeData card, Maybe<Member> owner, ResourceQuantity xCost)
    {
        var desc = card.Description;

        try
        {
            if (card.Actions() == null || card.Actions().Length < 0)
                return desc;

            var battleEffects = card.Actions()
                .SelectMany(a => a.BattleEffects);

            var conditionalBattleEffects = card.Actions()
                .SelectMany(a => a.Actions.Where(c => c.Type == CardBattleActionType.Condition))
                .SelectMany(b => b.ConditionData.ReferencedEffect.BattleEffects);
                        
            return InterpolatedDescription(desc, battleEffects.Concat(conditionalBattleEffects).ToArray(), card.ReactionBattleEffects().ToArray(), owner, xCost, card.ChainedCard);
        }
        catch (Exception e)
        {
            Log.Error($"Unable to Generate Interpolated Description for {card.Name}");
            Log.Error(e);
            #if UNITY_EDITOR
            throw;
            #endif
            return desc;
        }
    }

    public static string InterpolatedDescription(string desc, 
        EffectData[] effects, 
        EffectData[] reactionEffects, 
        Maybe<Member> owner, 
        ResourceQuantity xCost,
        Maybe<CardTypeData> chainedCard)
    {
        var result = desc;

        if (desc.Trim().Equals("{Auto}", StringComparison.InvariantCultureIgnoreCase))
        {
            var sb = new StringBuilder();
            sb.Append(AutoDescription(effects, owner, xCost));
            sb = new StringBuilder(ShortenRepeatedEffects(sb.ToString()));
            sb.Append(chainedCard.Select(c => $". {Bold("Chain:")} {c.Name}", ""));
            return sb.ToString();
        }

        var xCostReplacementToken = "{X}";
        result = result.Replace(xCostReplacementToken, Bold(XCostDescription(owner, xCost)));
        
        foreach (var r in _resourceIcons)
            result = result.Replace(r.Key, Sprite(r.Value));
        
        var tokens = Regex.Matches(result, "{(.*?)}");
        foreach (Match token in tokens)
        {
            var forReaction = token.Value.StartsWith("{RE[");
            var prefixes = new[] {"{E", "{D", "{RE"};
            if (prefixes.None(p => token.Value.StartsWith(p)))
                throw new InvalidDataException($"Unable to interpolate for things other than Battle Effects, Durations, and Reaction Effects");

            var effectIndex = int.Parse(Regex.Match(token.Result("$1"), "\\[(.*?)\\]").Result("$1"));
            if (!forReaction && effectIndex >= effects.Length)
                throw new InvalidDataException($"Requested Interpolating {effectIndex}, but only found {effects.Length} Battle Effects");
            if (forReaction && effectIndex >= reactionEffects.Length)
                throw new InvalidDataException($"Requested Interpolating {effectIndex}, but only found {reactionEffects.Length} Reaction Battle Effects");

            if (token.Value.StartsWith("{E["))
                result = result.Replace("{E[" + effectIndex + "]}", Bold(EffectDescription(effects[effectIndex], owner, xCost)));
            if (token.Value.StartsWith("{D["))
                result = result.Replace("{D[" + effectIndex + "]}", DurationDescription(effects[effectIndex]));
            if (forReaction)
                result = result.Replace("{RE[" + effectIndex + "]}", Bold(EffectDescription(reactionEffects[effectIndex], owner, xCost)));
        }

        return result;
    }

    private static string XCostDescription(Maybe<Member> owner, ResourceQuantity xCost)
        => ResourceQuantity.None == xCost
            ? owner.Select(
                o => o.State.PrimaryResourceAmount.ToString(),
                () => "X")
            : xCost.Amount.ToString();

    private static string AutoDescription(IEnumerable<EffectData> effects, Maybe<Member> owner, ResourceQuantity xCost)
        => string.Join(". ", effects.Select(e => AutoDescription(e, owner, xCost)));
    
    private static string AutoDescription(EffectData data, Maybe<Member> owner, ResourceQuantity xCost)
    {
        var delay = DelayDescription(data);
        var coreDesc = "";
        if (data.EffectType == EffectType.AttackFormula)
            coreDesc = $"deal {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.DealRawDamageFormula)
            coreDesc = $"deal {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.MagicAttackFormula)
            coreDesc = $"deal {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.DamageOverTimeFormula)
            coreDesc = $"deal {Bold(EffectDescription(data, owner, xCost))} {DurationDescription(data)}";
        if (data.EffectType == EffectType.AdjustCounterFormula)
            coreDesc = GivesOrRemoves(Bold(EffectDescription(data, owner, xCost)));
        if (data.EffectType == EffectType.AdjustStatAdditivelyFormula)
            coreDesc = $"gives {Bold(EffectDescription(data, owner, xCost))} {data.EffectScope.Value.WithSpaceBetweenWords()} {DurationDescription(data)}";
        if (data.EffectType == EffectType.AdjustStatMultiplicativelyFormula)
            coreDesc = $"gives {Bold(EffectDescription(data, owner, xCost))} {data.EffectScope.Value.WithSpaceBetweenWords()} {DurationDescription(data)}";
        if (data.EffectType == EffectType.ApplyVulnerable)
            coreDesc = $"gives {Bold("Vulnerable")} {DurationDescription(data)}";
        if (data.EffectType == EffectType.AdjustResourceFlat)
            coreDesc = $"gives {Bold(EffectDescription(data, owner, xCost))} {data.EffectScope}";
        if (data.EffectType == EffectType.AdjustPrimaryResourceFormula)
            coreDesc = $"gives {Bold(EffectDescription(data, owner, xCost))}"; ;
        if (data.EffectType == EffectType.ReactWithEffect)
            coreDesc = $"{WithCommaIfPresent(DurationDescription(data))}" +
                       $"{Bold(data.ReactionConditionType.ToString().WithSpaceBetweenWords())}: " +
                       $"{ReactionSourceDescription(owner, data.ReactionEffect.Reactor)}" +
                       $"{AutoDescription(data.ReactionEffect.CardActions.BattleEffects, owner, ResourceQuantity.None)} " +
                       $"to {ReactiveTargetFriendlyName(data.ReactionEffect.Scope)}";
        if (data.EffectType == EffectType.ReactWithCard)
            coreDesc = $"{WithCommaIfPresent(DurationDescription(data))}" +
                       $"{Bold(data.ReactionConditionType.ToString().WithSpaceBetweenWords())}: " +
                       $"{ReactionSourceDescription(owner, data.ReactionSequence.ActionSequence.Reactor)}" +
                       $"{AutoDescription(data.ReactionSequence.ActionSequence.CardActions.BattleEffects, owner, ResourceQuantity.None)} " +
                       $"to {ReactiveTargetFriendlyName(data.ReactionSequence.ActionSequence.Scope)}";
        if (data.EffectType == EffectType.ShieldFormula)
            coreDesc = $"gives {Bold(EffectDescription(data, owner, xCost))} Shield";
        if (data.EffectType == EffectType.DrawCards)
            coreDesc = $"draw {Bold(EffectDescription(data, owner, xCost))} Cards";
        if (data.EffectType == EffectType.EnterStealth)
            coreDesc = $"enter {Bold(TemporalStatType.Stealth.ToString())}";
        if (data.EffectType == EffectType.AdjustPlayerStats)
            coreDesc = $"{WithCommaIfPresent(DurationDescription(data))}" 
                       + $"gives {Bold(EffectDescription(data, owner, xCost))} " 
                       + $"{Bold(data.EffectScope.ToString().WithSpaceBetweenWords())}";
        if (data.EffectType == EffectType.AntiHeal)
            coreDesc = $"halves healing {DurationDescription(data)}";
        if (coreDesc == "")
            throw new InvalidDataException($"Unable to generate Auto Description for {data.EffectType}");
        return delay.Length > 0 
            ? $"{delay}{coreDesc}".Replace("Next turn, for the turn,", "Next turn,")
            : UppercaseFirst(coreDesc);
    }

    private static string ShortenRepeatedEffects(string value)
    {
        var modifiedValue = value + ". ";
        var repeatIndex = (modifiedValue + modifiedValue).IndexOf(value, 1);
        var repeatedValue = modifiedValue.Substring(0, repeatIndex);
        var repetitions = modifiedValue.Length / repeatedValue.Length;
        return repetitions > 1
            ? $"<b>{repetitions} Times:</b> {repeatedValue.Substring(0, repeatedValue.Length - 2)}"
            : value;
    }

    private static string WithCommaIfPresent(string value) => string.IsNullOrWhiteSpace(value) ? "" : $"{value}, ";
    
    private static string GivesOrRemoves(string remainingEffectDesc) 
        => remainingEffectDesc.Contains("-") || remainingEffectDesc.Contains("All") 
            ? $"removes {remainingEffectDesc}" 
            : $"gives {remainingEffectDesc}";
    
    private static string ReactionSourceDescription(Maybe<Member> owner, ReactiveMember member) 
        => member == ReactiveMember.Originator 
            ? owner.Select(o => o.Name + " will ", "Originator will ") 
            : "";
    
    private static string ReactiveTargetFriendlyName(ReactiveTargetScope s) 
        => s == ReactiveTargetScope.Source 
            ? "attacker" 
            : s.ToString().WithSpaceBetweenWords().ToLower();
    
    private static string UppercaseFirst(string s) => char.ToUpper(s[0]) + s.Substring(1);

    private static Dictionary<string, int> _resourceIcons = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase)
    {
        { "Ammo", 4},
        { "Chems", 5},
        { "Energy", 6},
        { "Flames", 7},
        { "Mana", 8},
        { "Tech Points", 9},
    };
    private static string PhysDamageIcon => Sprite(0);
    private static string RawDamageIcon => Sprite(1);
    private static string MagicDamageIcon => Sprite(2);
    private static string Sprite(int index) => $"<sprite index={index}>";

    private static string WithPhysicalDamageIcon(string s) => $"{s} {PhysDamageIcon}";
    private static string WithMagicDamageIcon(string s) => $"{s} {MagicDamageIcon}";
    private static string WithRawDamageIcon(string s) => $"{s} {RawDamageIcon}";
    
    public static string EffectDescription(EffectData data, Maybe<Member> owner, ResourceQuantity xCost)
    {
        if (data.EffectType == EffectType.AttackFormula)
            return WithPhysicalDamageIcon(FormulaAmount(data, owner, xCost));
        if (data.EffectType == EffectType.MagicAttackFormula)
            return WithMagicDamageIcon(FormulaAmount(data, owner, xCost));
        if (data.EffectType == EffectType.DamageOverTimeFormula)
            return WithRawDamageIcon(FormulaAmount(data, owner, xCost));
        if (data.EffectType == EffectType.DealRawDamageFormula)
            return WithRawDamageIcon(FormulaAmount(data, owner, xCost));
        if (data.EffectType == EffectType.AdjustStatAdditivelyFormula
                || data.EffectType == EffectType.HealFormula)
            return FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.AdjustStatMultiplicativelyFormula)
            return $"x{FormulaAmount(data, owner, xCost)}";
        if (data.EffectType == EffectType.HealOverTime)
            return MagicAmount(data, owner);
        if (data.EffectType == EffectType.DrawCards)
            return FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.ShieldFormula)
            return FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.AdjustCounterFormula)
            return $"{FormulaAmount(data, owner, xCost)} {Bold(data.EffectScope.Value.WithSpaceBetweenWords())}";
        if (data.EffectType == EffectType.AdjustPrimaryResourceFormula)
            return $"{FormulaAmount(data, owner, xCost)} Primary Resource";
        if (data.EffectType == EffectType.ShieldToughnessBasedOnNumberOfOpponentDoTs)
            return owner.IsPresent
                ? RoundUp(Mathf.Min(owner.Value.MaxShield(),(data.FloatAmount * owner.Value.State[StatType.Toughness]))).ToString()
                : $"{data.FloatAmount}x TGH";
        if (data.EffectType == EffectType.ApplyAdditiveStatInjury)
            return $"{data.FloatAmount} {data.EffectScope}";
        if (data.EffectType == EffectType.ApplyMultiplicativeStatInjury)
            return $"{data.FloatAmount}x {data.EffectScope}";
        if (data.EffectType == EffectType.AdjustResourceFlat)
            return $"{WithImplications(data.TotalIntAmount.ToString())}";
        if (data.EffectType == EffectType.AdjustPlayerStats)
            return $"{data.TotalIntAmount}";

        Log.Warn($"Description for {data.EffectType} is not implemented.");
        return "%%";
    }

    private static string FormulaAmount(EffectData data, Maybe<Member> owner, ResourceQuantity xCost)
        => WithImplications(owner.IsPresent
            ? RoundUp(Formula.Evaluate(owner.Value.State.ToSnapshot(), data.Formula, xCost)).ToString()
            : FormattedFormula(data.Formula));
    
    private static string MagicAmount(EffectData data, Maybe<Member> owner) 
        => WithImplications(owner.IsPresent
            ? RoundUp(data.BaseAmount + data.FloatAmount * owner.Value.State[StatType.Magic]).ToString()
            : WithBaseAmount(data, "x MAG"));

    private static string AttackAmount(EffectData data, Maybe<Member> owner)
        => WithImplications(owner.IsPresent
            ? RoundUp(data.BaseAmount + data.FloatAmount * owner.Value.State[StatType.Attack]).ToString()
            : data.BaseAmount > 0
                ? $"{data.BaseAmount} + {data.FloatAmount}x ATK"
                : $"{data.FloatAmount}x ATK");
    
    private static string WithBaseAmount(EffectData data, string floatString)
    {
        var baseAmount = data.BaseAmount != 0 ? $"{data.BaseAmount.Value}" : "";
        var floatAmount = data.FloatAmount > 0 ? $"{data.FloatAmount.Value}{floatString}" : "";
        return baseAmount + floatAmount;
    }

    public static string WithImplications(string value)
    {
        return value.Replace("-999", "All")
            .Replace("999", "Max");
    }

    private static string DurationDescription(EffectData data)
    {
        var value = data.NumberOfTurns.Value;
        var turnString = value < 0
                        ? "for the battle" 
                        : value < 2
                            ? data.TurnDelay == 0 ? "this turn" : "for the turn" 
                            : $"for {Bold(value.ToString())} turns";

        return $"{turnString}";
    }

    private static string DelayDescription(EffectData data)
    {
        var delayValue = data.TurnDelay;
        var delayString = delayValue < 1
            ? ""
            : delayValue == 1
                ? "Next turn, "
                : $"In {delayValue} turns, ";
        return delayString;
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

    private static string Standardized(string effectScope) => StatAbbreviations.ValueOrDefault(effectScope, () => effectScope);
    
    private static Dictionary<string, string> StatAbbreviations = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
        { StatType.Leadership.ToString(), "LEAD" },
        { StatType.Attack.ToString(), "ATK" },
        { StatType.Magic.ToString(), "MAG" },
        { StatType.Armor.ToString(), "ARM" },
        { StatType.Toughness.ToString(), "TGH" },
    }; 
}
