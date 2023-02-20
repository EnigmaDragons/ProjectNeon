using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using I2.Loc;
using UnityEngine;

public static class InterpolatedCardDescriptions
{
    private static int RoundUp(float f) => f > 0 ? Mathf.CeilToInt(f) : Mathf.FloorToInt(f);
    private static string Bold(this string s) => $"<b>{s}</b>";

    public static string InterpolatedDescription(this Card card, ResourceQuantity xCost) 
        => card.Type.InterpolatedDescription(card.Owner, xCost);

#if UNITY_EDITOR
    public static string EditorOnlyLibraryPreview(this CardTypeData card)
    {
        try
        {
            return card.InterpolatedDescription(Maybe<Member>.Missing(), ResourceQuantity.None, 0, 0, card.DescriptionV2.Preview());
        }
        catch (Exception)
        {
            return $"{card.Name} Description is not Translated Correctly into {LocalizationManager.CurrentLanguage}";
        }
    }
#endif

    public static string LocalizedDescription(this CardTypeData card, Maybe<Member> owner, ResourceQuantity xCost, int cardsInHand = 0, int cardsCycledThisTurn = 0)
    {
        var desc = card.Description;
        try
        {
            if (card.DescriptionV2.IsUsable())
                desc = Localized.FormatTerm(card.DescriptionTerm, card.DescriptionV2.formatArgs);
        }
        catch (Exception)
        {
            return $"{card.Name} Description is not Translated Correctly into {LocalizationManager.CurrentLanguage}";
        }

        return card.InterpolatedDescription(owner, xCost, cardsInHand, cardsCycledThisTurn, desc);
    }

    public static string InterpolatedDescription(this CardTypeData card, Maybe<Member> owner, ResourceQuantity xCost, int cardsInHand = 0, int cardsCycledThisTurn = 0, string overrideDescription = null)
    {
        var desc = overrideDescription ?? card.Description;

        try
        {
            if (card.Actions() == null || card.Actions().Length < 0)
                return desc;

            var battleEffects = card.Actions()
                .SelectMany(a => a.BattleEffects);

            var conditionalBattleEffects = card.Actions()
                .SelectMany(a => a.Actions.Where(c => c.Type == CardBattleActionType.Condition))
                .SelectMany(b => b.ConditionData.ReferencedEffect.BattleEffects);

            var innerBattleEffects = card.Actions().SelectMany(c => c.InnerBattleEffects)
                .Concat(card.Actions()
                    .SelectMany(a => a.Actions.Where(c => c.Type == CardBattleActionType.Condition))
                    .SelectMany(b => b.ConditionData.ReferencedEffect.InnerBattleEffects));

            return InterpolatedDescription(desc, card.Speed == CardSpeed.Quick, battleEffects.Concat(conditionalBattleEffects).ToArray(), card.ReactionBattleEffects().ToArray(), innerBattleEffects.ToArray(), owner, xCost, card.ChainedCard, card.SwappedCard, cardsInHand, cardsCycledThisTurn);
        }
        catch (Exception e)
        {
            #if UNITY_EDITOR
            Log.Error($"Unable to Generate Interpolated Description for {card.Name} {card.Id}");
            Log.Error(e);
            throw;
            #else
            Log.Error($"Unable to Generate Interpolated Description for {card.Name}");
            Log.Error(e);
            return desc;
            #endif
        }
    }

    public static string InterpolatedDescription(string desc, 
        bool isQuick,
        EffectData[] effects, 
        EffectData[] reactionEffects,
        EffectData[] innerEffects,
        Maybe<Member> owner, 
        ResourceQuantity xCost,
        Maybe<CardTypeData> chainedCard,
        Maybe<CardTypeData> swappedCard,
        int cardsInHand,
        int cardCyclesUsedThisTurn)
    {
        var result = desc;

        if (desc.Trim().Equals("{Auto}", StringComparison.InvariantCultureIgnoreCase))
        {
            var sb = new StringBuilder();
            if (isQuick)
                sb.Append("<b>Quick:</b> ");
            sb.Append(AutoDescription(effects, owner, xCost));
            sb = new StringBuilder(ShortenRepeatedEffects(sb.ToString()));
            sb.Append(chainedCard.Select(c => $". {Bold("Finisher:")} {c.Name}", string.Empty));
            sb.Append(swappedCard.Select(c => $". {Bold("Swap:")} {c.Name}", string.Empty));
            return sb.ToString();
        }
        
        if (desc.Trim().Equals("{AutoPartial}", StringComparison.InvariantCultureIgnoreCase))
        {
            var sb = new StringBuilder();
            if (isQuick)
                sb.Append("<b>Quick:</b> ");
            sb.Append(AutoPartial(effects));
            sb = new StringBuilder(ShortenRepeatedEffects(sb.ToString()));
            sb.Append(chainedCard.Select(c => $". {Bold("Finisher:")} {c.Name}", string.Empty));
            sb.Append(swappedCard.Select(c => $". {Bold("Swap:")} {c.Name}", string.Empty));
            return sb.ToString();
        }

        var xCostReplacementToken = "{X}";
        result = result.Replace(xCostReplacementToken, Bold(XCostDescription(owner, xCost)));

        var ownerReplacementToken = "{Owner}";
        result = result.Replace(ownerReplacementToken, owner.Select(o => o.NameTerm.ToLocalized(), () => "Owner"));
        
        var tokens = Regex.Matches(result, "{(.*?)}");
        foreach (Match token in tokens)
        {
            var forReaction = token.Value.StartsWith("{RE[");
            var forInnerEffect = token.Value.StartsWith("{I");
            var prefixes = new[] {"{E", "{D", "{RE", "{RD", "{IE", "{ID", "{ES", "{F" };
            if (prefixes.None(p => token.Value.StartsWith(p)))
                throw new InvalidDataException($"Unable to interpolate for things other than Battle Effects, Durations, and Reaction Effects");

            var effectIndex = int.Parse(Regex.Match(token.Result("$1"), "\\[(.*?)\\]").Result("$1"));
            if (!forReaction && !forInnerEffect && effectIndex >= effects.Length)
                throw new InvalidDataException($"Requested Interpolating {effectIndex}, but only found {effects.Length} Battle Effects");
            if (forReaction && effectIndex >= reactionEffects.Length)
                throw new InvalidDataException($"Requested Interpolating {effectIndex}, but only found {reactionEffects.Length} Reaction Battle Effects");
            if (forInnerEffect && effectIndex >= innerEffects.Length)
                throw new InvalidDataException($"Requested Interpolating Inner Effect {effectIndex}, but only found {innerEffects.Length} Inner Effects");

            if (token.Value.StartsWith("{E["))
                result = result.Replace("{E[" + effectIndex + "]}", Bold(EffectDescription(effects[effectIndex], owner, xCost, cardsInHand, cardCyclesUsedThisTurn)));
            if (token.Value.StartsWith("{ES["))
                result = result.Replace("{ES[" + effectIndex + "]}", Bold(EffectDescription(effects[effectIndex], owner, xCost, cardsInHand, cardCyclesUsedThisTurn, false)));
            if (token.Value.StartsWith("{D["))
                result = result.Replace("{D[" + effectIndex + "]}", DurationDescription(effects[effectIndex], owner, xCost));
            if (token.Value.StartsWith("{F["))
                result = result.Replace("{F[" + effectIndex + "]}", Bold(effects[effectIndex].Formula));
            if (forReaction)
                result = result.Replace("{RE[" + effectIndex + "]}", Bold(EffectDescription(reactionEffects[effectIndex], owner, xCost, cardsInHand, cardCyclesUsedThisTurn)));
            if (token.Value.StartsWith("{RD["))
                result = result.Replace("{RD[" + effectIndex + "]}", DurationDescription(reactionEffects[effectIndex], owner, xCost));
            if (token.Value.StartsWith("{IE"))
                result = result.Replace("{IE[" + effectIndex + "]}", Bold(EffectDescription(innerEffects[effectIndex], owner, xCost, cardsInHand, cardCyclesUsedThisTurn)));
            if (token.Value.StartsWith("{ID"))
                result = result.Replace("{ID[" + effectIndex + "]}", DurationDescription(innerEffects[effectIndex], owner, xCost));
        }

        if (owner.IsPresent && _resourceIcons.TryGetValue(owner.Value.PrimaryResourceQuantity().ResourceType, out var icon))
        {
            result = result.Replace("Owner[PrimaryResource]", Sprite(icon));
            result = result.Replace("GlobalPrimaryResource", Sprite(icon));
        }

        result = result.Replace("GlobalPrimaryResource", "Resources");
        result = result.Replace("POW", "Power");
        result = result.Replace("PrimaryStat", "Power");
        
        foreach (var r in _resourceIcons)
        {
            result = Regex.Replace(result, $" {r.Key}", $" {Sprite(r.Value)}");
            result = Regex.Replace(result, $"{r.Key}", $"{Sprite(r.Value)}");
        }

        result = Regex.Replace(result, @"@(\w+)", "$1");
        
        // Add Line Breaks
        result = result.Replace("\\n", "\n");
        
        return result;
    }

    private const string X = "X";
    private static string XCostDescription(Maybe<Member> owner, ResourceQuantity xCost)
    {
        if (xCost.Amount == -1)
            return X;
        if (ResourceQuantity.None == xCost)
            return owner.Select(o => o.State.PrimaryResourceAmount.ToString(), () => X);
        return xCost.Amount.ToString();
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

    private static string AOrAn(string archetype) => "aeiouAEIOU".IndexOf(archetype[0]) >= 0 ? "an" : "a";
    private static string WithCommaIfPresent(string value) => string.IsNullOrWhiteSpace(value) ? string.Empty : $"{value}, ";
    
    private static string GivesOrRemoves(string remainingEffectDesc) 
        => remainingEffectDesc.Contains("-") || remainingEffectDesc.Contains("All") 
            ? $"removes {remainingEffectDesc}" 
            : $"gives {remainingEffectDesc}";
    
    private static string GivesOrRemovesPartial(string effectPart, string remainingEffectDesc) 
        => remainingEffectDesc.Contains("-") || remainingEffectDesc.Contains("All") 
            ? $"removes {effectPart}" 
            : $"gives {effectPart}";
    
    private static string ReactiveTargetFriendlyName(ReactiveTargetScope s) 
        => s == ReactiveTargetScope.Source 
            ? "attacker" 
            : s.ToString().WithSpaceBetweenWords().ToLower();
    
    private static string UppercaseFirst(string s) => char.ToUpper(s[0]) + s.Substring(1);

    private static Dictionary<string, int> _resourceIcons = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase)
    {
        { "Ammo", 4 },
        { "Chems", 5 },
        { "Energy", 6 },
        { "Flames", 7 },
        { "Mana", 20 },
        { "Insight", 24 },
        { "Tech Points", 23 },
        { "TechPoints", 23 },
        { "PrimaryResource", 6 },
        { "Primary Resource", 6 },
        { "Grenades", 8 },
        { "Grenade", 8 },
        { "Ambition", 9 },
        { "Credits", 21},
        { "Creds", 21},
    };
    public static string PhysDamageIcon => Sprite(0);
    public static string TrueDamageIcon => Sprite(1);
    public static string MagicDamageIcon => Sprite(2);
    private static string Sprite(int index) => $"<sprite index={index}>";
    
    public static string WithPhysicalDamageIcon(string s) => $"{s.PhysDamageColored()} {PhysDamageIcon}";
    public static string WithMagicDamageIcon(string s) => $"{s.MagicDamageColored()} {MagicDamageIcon}";
    public static string WithTrueDamageIcon(string s) => $"{s.TrueDamageColored()} {TrueDamageIcon}";
    
    public static string EffectDescription(EffectData data, Maybe<Member> owner, ResourceQuantity xCost, int cardsInHand = 0, int cardCyclesUsedThisTurn = 0, bool showSprites = true)
    {
        if (data.EffectType == EffectType.AttackFormula)
            return showSprites ? WithPhysicalDamageIcon(FormulaAmount(data, owner, xCost)) : FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.MagicAttackFormula)
            return showSprites ? WithMagicDamageIcon(FormulaAmount(data, owner, xCost)) : FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.TrueDamageAttackFormula)
            return showSprites ? WithTrueDamageIcon(FormulaAmount(data, owner, xCost)) : FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.DamageOverTimeFormula)
            return showSprites ? WithTrueDamageIcon(FormulaAmount(data, owner, xCost)) : FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.DealTrueDamageFormula)
            return showSprites ? WithTrueDamageIcon(FormulaAmount(data, owner, xCost)) : FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.HealFormula 
                || data.EffectType == EffectType.AdjustPlayerStatsFormula
                || data.EffectType == EffectType.ChooseCardToCreate)
            return FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.AdjustStatAdditivelyFormula)
            return FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.AdjustStatMultiplicativelyFormula)
            return $"× {FormulaAmount(data, owner, xCost)}";
        if (data.EffectType == EffectType.HealOverTime)
            return MagicAmount(data, owner);
        if (data.EffectType == EffectType.DrawCards)
            return FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.DrawCardsOfOwner)
            return FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.ShieldFormula)
            return FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.AdjustCounterFormula)
            return $"{FormulaAmount(data, owner, xCost)} {FriendlyScopeName(data.EffectScope.Value)}";
        if (data.EffectType == EffectType.AdjustPrimaryResourceFormula)
            return $"{FormulaAmount(data, owner, xCost)} {(owner.IsPresent && showSprites ? owner.Value.PrimaryResourceQuantity().ResourceType : "Resources")}";
        if (data.EffectType == EffectType.ShieldBasedOnNumberOfOpponentsDoTs)
            return owner.IsPresent
                ? RoundUp(Mathf.Min(owner.Value.MaxShield(),(data.FloatAmount * owner.Value.State[StatType.MaxShield]))).ToString()
                : $"{data.FloatAmount} × MaxShield";
        if (data.EffectType == EffectType.ApplyAdditiveStatInjury)
            return $"{data.FloatAmount} {data.EffectScope}";
        if (data.EffectType == EffectType.ApplyMultiplicativeStatInjury)
            return $"{data.FloatAmount} × {data.EffectScope}";
        if (data.EffectType == EffectType.AdjustResourceFlat)
            return $"{WithImplications(data.TotalIntAmount.ToString())} {data.EffectScope.Value.WithSpaceBetweenWords()}";
        if (data.EffectType == EffectType.AdjustPlayerStats)
            return $"{data.TotalIntAmount}";
        if (data.EffectType == EffectType.TransferPrimaryResourceFormula)
            return FormulaAmount(data, owner, xCost);
        if (data.EffectType == EffectType.GainCredits)
            return data.BaseAmount.ToString();
        if (data.EffectType == EffectType.DisableForTurns)
            return Bold("Disabled");
        if (data.EffectType == EffectType.Drain)
            return $"{Bold(FormulaAmount(data, owner, xCost))} {data.EffectScope.Value.WithSpaceBetweenWords()}";
        if (data.EffectType == EffectType.EnterStealth)
            return $"Enter {Bold("Stealth")}";
        if (data.EffectType == EffectType.AdjustPrimaryStatForEveryCardCycledAndInHand)
            return cardsInHand + cardCyclesUsedThisTurn - 1 > 0 ? $"{cardsInHand + cardCyclesUsedThisTurn - 1}" : string.Empty;
        
        Log.Warn($"Description for {data.EffectType} is not implemented.");
        return "%%";
    }
    
    private static string FriendlyScopeName(string raw)
    {
        var tmp = raw;
        if (StatTypeAliases.FullNameToAbbreviations.TryGetValue(raw, out var friendly))
            tmp = friendly;
        if (TemporalStatFriendlyNames.TryGetValue(raw, out var friendly2))
            tmp = friendly2;
        return tmp.WithSpaceBetweenWords();
    }

    private static string FormulaAmount(EffectData data, Maybe<Member> owner, ResourceQuantity xCost)
        => WithImplications(owner.IsPresent && (!data.Formula.Contains('X') || xCost.ResourceType != "None")
            ? EvaluatedFormula(data, owner.Value, xCost)
            : FormattedFormula(data));

    private static string EvaluatedFormula(EffectData data, Member owner, ResourceQuantity xCost)
    {
        var f = data.InterpolateFriendlyFormula();
        if (f.ShouldUsePartialFormula)
        {
            var ipf = f.InterpolatePartialFormula;
            var formulaResult = ipf.EvaluationPartialFormula.Length > 0
                ? FormulaResult(ipf.EvaluationPartialFormula, owner, xCost).ToString("0.##")
                : string.Empty;
            formulaResult = formulaResult.Equals("0") ? string.Empty : formulaResult;
            return FormattedFormula($"{ipf.Prefix} {formulaResult} {ipf.Suffix}".Trim())
                    .Replace("Owner[PrimaryResource]", owner.PrimaryResourceQuantity().ResourceType)
                    .Replace("PrimaryResource", owner.PrimaryResourceQuantity().ResourceType);
        }

        return RoundUp(FormulaResult(f.FullFormula, owner, xCost)).ToString();
    }

    private static int FormulaResult(string formula, Member owner, ResourceQuantity xCost)
        => Formula.EvaluateToInt(owner.State.ToSnapshot(), formula, xCost, new EffectScopedData());
    
    private static string MagicAmount(EffectData data, Maybe<Member> owner) 
        => WithImplications(owner.IsPresent
            ? RoundUp(data.BaseAmount + data.FloatAmount * owner.Value.State[StatType.Magic]).ToString()
            : WithBaseAmount(data, " × MAG"));
    
    private static string WithBaseAmount(EffectData data, string floatString)
    {
        var baseAmount = data.BaseAmount != 0 ? $"{data.BaseAmount.Value}" : string.Empty;
        var floatAmount = data.FloatAmount > 0 ? $"{data.FloatAmount.Value}{floatString}" : string.Empty;
        return baseAmount + floatAmount;
    }

    public static string WithImplications(string value)
    {
        return value.Replace("-999", "All")
            .Replace("999", "Max");
    }

    private static string Dur_ForNTurns = "Cards/Interp_ForNTurns";
    private static string Eng_ForNTurns = "for {0} turns";
    private static string Dur_ForTheBattle = "Cards/Interp_ForTheBattle";
    private static string Eng_ForTheBattle = "for the battle";
    private static string Dur_ThisTurn = "Cards/Interp_ForThisTurn";
    private static string Eng_ThisTurn = "this turn";
    private static string Dur_ForTheTurn = "Cards/Interp_ForTheTurn";
    private static string Eng_ForTheTurn = "for the turn";
    
    private static string DurationDescription(EffectData data, Maybe<Member> owner, ResourceQuantity xCost)
    {
        var value = -1;
        var formulaValueString = owner.IsMissing ? Bold(FormattedFormula(data.DurationFormula)) : string.Empty;
        if (owner.IsPresent)
        {
            value = Formula.EvaluateToInt(owner.Value.State.ToSnapshot(), data.DurationFormula, xCost, new EffectScopedData());
            formulaValueString = Bold(value.ToString());
        }

        if (owner.IsPresent && value < 0)
            return Localized.StringTermOrDefault(Dur_ForTheBattle, Eng_ForTheBattle);
        
        if (value == 1) 
            if (data.TurnDelay == 0)
                return Localized.StringTermOrDefault(Dur_ThisTurn, Eng_ThisTurn); 
            else
                return Localized.StringTermOrDefault(Dur_ForTheTurn, Eng_ForTheTurn); 

        return Localized.FormatTermOrDefault(Dur_ForNTurns, Eng_ForNTurns, formulaValueString);
    }

    private static string DelayDescription(EffectData data)
    {
        var delayValue = data.TurnDelay;
        var delayString = delayValue < 1
            ? string.Empty
            : delayValue == 1
                ? "Next turn, "
                : $"In {delayValue} turns, ";
        return delayString;
    }

    private static string FormattedFormula(EffectData data)
    {
        var f = data.InterpolateFriendlyFormula();
        if (f.ShouldUsePartialFormula)
        {
            var ipf = f.InterpolatePartialFormula;
            return FormattedFormula($"{ipf.Prefix} {FormattedFormula(ipf.EvaluationPartialFormula)} {ipf.Suffix}".Trim());
        }

        return FormattedFormula(data.Formula);
    }

    private static string StarMultiplySymbol = " * ";
    private static string TimesMuliplySymbol = " × ";
    
    private static string FormattedFormula(string s)
    {
        var newS = s;
        newS = newS.Replace(StarMultiplySymbol, TimesMuliplySymbol);
        foreach (var stat in StatTypeAliases.FullNameToAbbreviations)
            if (newS.Contains(stat.Key))
                newS = newS.Replace(stat.Key, stat.Value);

        return newS;
    }
    
    private static Dictionary<string, string> TemporalStatFriendlyNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
        {TemporalStatType.Marked.ToString(), "Mark"},
    };
    
#region Auto

    public static string AutoDescription(this CardTypeData card, Maybe<Member> owner, ResourceQuantity xCost)
        => InterpolatedDescription(card, owner, xCost, 0, 0, "{Auto}");

    public static string AutoPartial(this CardTypeData card)
        => InterpolatedDescription(card, Maybe<Member>.Missing(), ResourceQuantity.None, 0, 0, "{AutoPartial}");

    private static string AutoPartial(IEnumerable<EffectData> effects)
    {
        if (effects.Any(e => e.EffectType == EffectType.ResolveInnerEffect))
            throw new Exception("Should Not Partialized Automatically");
        return string.Join(". ", effects.Select((e, i) => AutoPartial(i, e)));
    }

    private static string AutoPartial(int index, EffectData data)
    {
        var delay = DelayDescription(data);
        var e = "{E[" + index +"]}";
        var d = "{D[" + index + "]}";
        var coreDesc = string.Empty;
        if (data.EffectType == EffectType.AttackFormula)
            coreDesc = $"deal {e}";
        if (data.EffectType == EffectType.TrueDamageAttackFormula)
            coreDesc = $"deal {e}";
        if (data.EffectType == EffectType.HealFormula)
            coreDesc = $"heal {e}";
        if (data.EffectType == EffectType.DealTrueDamageFormula)
            coreDesc = $"deal {e}";
        if (data.EffectType == EffectType.MagicAttackFormula)
            coreDesc = $"deal {e}";
        if (data.EffectType == EffectType.DamageOverTimeFormula)
            coreDesc = $"deal {e} {d}";
        if (data.EffectType == EffectType.AdjustCounterFormula)
            coreDesc = GivesOrRemoves(e);
        if (data.EffectType == EffectType.AdjustStatAdditivelyFormula)
            coreDesc = $"gives {e} {d}";
        if (data.EffectType == EffectType.AdjustStatMultiplicativelyFormula)
            coreDesc = $"gives {e} {d}";
        if (data.EffectType == EffectType.AdjustResourceFlat)
            coreDesc = $"gives {e}";
        if (data.EffectType == EffectType.AdjustPrimaryResourceFormula)
            coreDesc = $"gives {e}";
        if (data.EffectType == EffectType.DisableForTurns)
            coreDesc = $"gives {e}";
        if (data.EffectType == EffectType.ReactWithEffect)
            coreDesc = $"{WithCommaIfPresent(d)}" +
                       $"{Bold(data.ReactionConditionType.ToString().WithSpaceBetweenWords())}: " +
                       $"{AutoReactionSourceDescription(Maybe<Member>.Missing(), data.ReactionEffect.Reactor)}" +
                       $"{AutoPartial(data.ReactionEffect.CardActions.BattleEffects)} " +
                       $"to {ReactiveTargetFriendlyName(data.ReactionEffect.Scope)}";
        if (data.EffectType == EffectType.ReactWithCard)
            coreDesc = $"{WithCommaIfPresent(d)}" +
                       $"{Bold(data.ReactionConditionType.ToString().WithSpaceBetweenWords())}: " +
                       $"{AutoReactionSourceDescription(Maybe<Member>.Missing(), data.ReactionSequence.ActionSequence.Reactor)}" +
                       $"{AutoPartial(data.ReactionSequence.ActionSequence.CardActions.BattleEffects)} " +
                       $"to {ReactiveTargetFriendlyName(data.ReactionSequence.ActionSequence.Scope)}";
        if (data.EffectType == EffectType.ShieldFormula)
            coreDesc = $"shield {e}";
        if (data.EffectType == EffectType.DrawCards)
            coreDesc = $"draw {e} Cards";
        if (data.EffectType == EffectType.DrawCardsOfOwner)
            coreDesc = $"draw {e} of your Cards";
        if (data.EffectType == EffectType.EnterStealth)
            coreDesc = $"enter {Bold(TemporalStatType.Stealth.ToString())}";
        if (data.EffectType == EffectType.AdjustPlayerStats)
            coreDesc = $"{WithCommaIfPresent(d)}" 
                       + $"gives {e} "
                       + $"{Bold(data.EffectScope.ToString().WithSpaceBetweenWords())}";
        if (data.EffectType == EffectType.RemoveDebuffs)
            coreDesc = "removes all debuffs";
        if (data.EffectType == EffectType.AdjustPlayerStatsFormula)
            coreDesc = $"{WithCommaIfPresent(d)}" 
                       + $"gives {e} " 
                       + $"{Bold(data.EffectScope.ToString().WithSpaceBetweenWords())}";
        if (data.EffectType == EffectType.GainCredits)
            coreDesc = $"gives {Bold($"{data.BaseAmount} Creds")}";
        if (data.EffectType == EffectType.ChooseAndDrawCardOfArchetype)
            coreDesc = $"choose and draw {AOrAn(data.EffectScope)} {Bold(data.EffectScope.ToString().WithSpaceBetweenWords())} card";
        if (data.EffectType == EffectType.Drain)
            coreDesc = $"drain {e}";
        if (data.EffectType == EffectType.ShieldRemoveAll)
            coreDesc = $"removes all shields";
        if (coreDesc == string.Empty)
            throw new InvalidDataException($"Unable to generate Auto Description for {data.EffectType}");
        return delay.Length > 0 
            ? $"{delay}{coreDesc}".Replace("Next turn, for the turn,", "Next turn,")
            : UppercaseFirst(coreDesc);
    }
    
    private static string AutoDescription(IEnumerable<EffectData> effects, Maybe<Member> owner, ResourceQuantity xCost) 
        => string.Join(". ", effects.Select(e => e.EffectType == EffectType.ResolveInnerEffect 
            ? AutoDescription(e.ReferencedSequence.BattleEffects, owner, xCost)
            : AutoDescription(e, owner, xCost)));
    
    private static string AutoDescription(EffectData data, Maybe<Member> owner, ResourceQuantity xCost)
    {
        var delay = DelayDescription(data);
        var coreDesc = string.Empty;
        if (data.EffectType == EffectType.AttackFormula)
            coreDesc = $"deal {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.TrueDamageAttackFormula)
            coreDesc = $"deal {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.HealFormula)
            coreDesc = $"heal {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.DealTrueDamageFormula)
            coreDesc = $"deal {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.MagicAttackFormula)
            coreDesc = $"deal {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.DamageOverTimeFormula)
            coreDesc = $"deal {Bold(EffectDescription(data, owner, xCost))} {DurationDescription(data, owner, xCost)}";
        if (data.EffectType == EffectType.AdjustCounterFormula)
            coreDesc = GivesOrRemoves(Bold(EffectDescription(data, owner, xCost)));
        if (data.EffectType == EffectType.AdjustStatAdditivelyFormula)
            coreDesc = $"gives {Bold(EffectDescription(data, owner, xCost))} {data.EffectScope.Value.WithSpaceBetweenWords()} {DurationDescription(data, owner, xCost)}";
        if (data.EffectType == EffectType.AdjustStatMultiplicativelyFormula)
            coreDesc = $"gives {Bold(EffectDescription(data, owner, xCost))} {data.EffectScope.Value.WithSpaceBetweenWords()} {DurationDescription(data, owner, xCost)}";
        if (data.EffectType == EffectType.AdjustResourceFlat)
            coreDesc = $"gives {Bold(EffectDescription(data, owner, xCost))} {data.EffectScope.Value}";
        if (data.EffectType == EffectType.AdjustPrimaryResourceFormula)
            coreDesc = $"gives {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.DisableForTurns)
            coreDesc = $"gives {Bold((EffectDescription(data, owner, xCost)))}";
        if (data.EffectType == EffectType.ReactWithEffect)
            coreDesc = $"{WithCommaIfPresent(DurationDescription(data, owner, xCost))}" +
                       $"{Bold(data.ReactionConditionType.ToString().WithSpaceBetweenWords())}: " +
                       $"{AutoReactionSourceDescription(owner, data.ReactionEffect.Reactor)}" +
                       $"{AutoDescription(data.ReactionEffect.CardActions.BattleEffects, owner, ResourceQuantity.None)} " +
                       $"to {ReactiveTargetFriendlyName(data.ReactionEffect.Scope)}";
        if (data.EffectType == EffectType.ReactWithCard)
            coreDesc = $"{WithCommaIfPresent(DurationDescription(data, owner, xCost))}" +
                       $"{Bold(data.ReactionConditionType.ToString().WithSpaceBetweenWords())}: " +
                       $"{AutoReactionSourceDescription(owner, data.ReactionSequence.ActionSequence.Reactor)}" +
                       $"{AutoDescription(data.ReactionSequence.ActionSequence.CardActions.BattleEffects, owner, ResourceQuantity.None)} " +
                       $"to {ReactiveTargetFriendlyName(data.ReactionSequence.ActionSequence.Scope)}";
        if (data.EffectType == EffectType.ShieldFormula)
            coreDesc = $"shield {Bold(EffectDescription(data, owner, xCost))}";
        if (data.EffectType == EffectType.DrawCards)
            coreDesc = $"draw {Bold(EffectDescription(data, owner, xCost))} Cards";
        if (data.EffectType == EffectType.DrawCardsOfOwner)
            coreDesc = $"draw {Bold(EffectDescription(data, owner, xCost))} of your Cards";
        if (data.EffectType == EffectType.EnterStealth)
            coreDesc = $"enter {Bold(TemporalStatType.Stealth.ToString())}";
        if (data.EffectType == EffectType.AdjustPlayerStats)
            coreDesc = $"{WithCommaIfPresent(DurationDescription(data, owner, xCost))}" 
                       + $"gives {Bold(EffectDescription(data, owner, xCost))} " 
                       + $"{Bold(data.EffectScope.ToString().WithSpaceBetweenWords())}";
        if (data.EffectType == EffectType.RemoveDebuffs)
            coreDesc = "removes all debuffs";
        if (data.EffectType == EffectType.AdjustPlayerStatsFormula)
            coreDesc = $"{WithCommaIfPresent(DurationDescription(data, owner, xCost))}" 
                       + $"gives {Bold(FormulaAmount(data, owner, xCost))} " 
                       + $"{Bold(data.EffectScope.ToString().WithSpaceBetweenWords())}";
        if (data.EffectType == EffectType.GainCredits)
            coreDesc = $"gives {Bold($"{data.BaseAmount} Creds")}";
        if (data.EffectType == EffectType.ChooseAndDrawCardOfArchetype)
            coreDesc = $"choose and draw {AOrAn(data.EffectScope)} {Bold(data.EffectScope.ToString().WithSpaceBetweenWords())} card";
        if (data.EffectType == EffectType.Drain)
            coreDesc = $"drain {EffectDescription(data, owner, xCost)}";
        if (data.EffectType == EffectType.ShieldRemoveAll)
            coreDesc = $"removes all shields";
        if (coreDesc == string.Empty)
            throw new InvalidDataException($"Unable to generate Auto Description for {data.EffectType}");
        return delay.Length > 0 
            ? $"{delay}{coreDesc}".Replace("Next turn, for the turn,", "Next turn,")
            : UppercaseFirst(coreDesc);
    }

    private static string AutoReactionSourceDescription(Maybe<Member> owner, ReactiveMember member) 
        => member == ReactiveMember.Originator 
            ? owner.Select(o => o.NameTerm.ToEnglish() + " will ", "Originator will ") 
            : string.Empty;
    
    #endregion
}
