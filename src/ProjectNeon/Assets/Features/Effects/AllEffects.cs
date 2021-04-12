using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public static class AllEffects
{
    private static readonly Dictionary<EffectType, Func<EffectData, Effect>> CreateEffectOfType = new Dictionary<EffectType, Func<EffectData, Effect>>
    {
        { EffectType.Nothing, e => new NoEffect() },
        { EffectType.AdjustStatAdditivelyFormula, e => new AdjustStatsFormula(e, multiplicative: false)},
        { EffectType.AdjustStatMultiplicativelyFormula, e => new AdjustStatsFormula(e, multiplicative: true)},
        { EffectType.ReactWithEffect, e => new EffectReactWith(false, e.IntAmount, e.NumberOfTurns, e.StatusDetail, e.ReactionEffectScope,
            ReactiveTriggerScopeExtensions.Parse(e.EffectScope), e.ReactionConditionType, e.ReactionEffect)},
        { EffectType.ReactWithCard, e => new EffectReactWith(false, e.IntAmount, e.NumberOfTurns, e.StatusDetail, e.ReactionEffectScope, 
            ReactiveTriggerScopeExtensions.Parse(e.EffectScope), e.ReactionConditionType, e.ReactionSequence)},
        { EffectType.RemoveDebuffs, e => new SimpleEffect(m => BattleLogged($"{m.Name} has been cleansed of all debuffs", m.CleanseDebuffs))},
        { EffectType.AdjustCounterFormula, e => new AdjustCounterFormula(e)},
        { EffectType.ShieldFormula, e => new AegisIfFormulaResult((ctx, amount, m) 
            => BattleLoggedItem(diff => $"{m.Name} {GainedOrLostTerm(diff)} {diff} Shield", m.AdjustShield(amount)), e.Formula, true, amount => amount < 0)},
        { EffectType.ShieldRemoveAll, e => new AegisPreventable(new SimpleEffect(m => BattleLogged($"{m.Name} lost all their shields", () => m.AdjustShield(-999))), "Losing All Shields") },
        { EffectType.ShieldToughnessBasedOnNumberOfOpponentDoTs, e => new ShieldToughnessBasedOnNumberOfOpponentDoTs(e.FloatAmount) },
        { EffectType.AdjustResourceFlat, e =>  AegisPreventable.If(new SimpleEffect(m => m.GainResource(e.EffectScope.Value, e.TotalIntAmount)), e.TotalIntAmount < 0, "Resource Loss") },
        { EffectType.AdjustPrimaryResourceFormula, e => new AegisIfFormulaResult((ctx, amount, m) => 
            m.AdjustPrimaryResource(BattleLoggedItem(v => $"{m.Name} {GainedOrLostTerm(v)} {v} {m.PrimaryResource.Name}", amount.CeilingInt())), e.Formula, true, amount => amount < 0) },
        { EffectType.DamageOverTimeFormula, e => new AegisPreventable(new DamageOverTimeFormula(e), "Damage Over Time") },
        { EffectType.ApplyVulnerable, e => new AegisPreventable(new SimpleEffect(m => BattleLogged($"{m.Name} has become vulnerable",
            () => m.ApplyTemporaryMultiplier(new AdjustedStats(new StatMultipliers().With(StatType.Damagability, 1.33f), 
                TemporalStateMetadata.DebuffForDuration(e.NumberOfTurns, new StatusDetail(StatusTag.Vulnerable)))))), "Vulernable") },
        { EffectType.DisableForTurns, e => new SimpleEffect(m => BattleLogged($"{m.Name} is disabled for {e.NumberOfTurns} turns.", () => m.ApplyTemporaryAdditive(new DisableForTurns(e.NumberOfTurns))))},
        { EffectType.HealOverTime, e => new HealOverTime(e.FloatAmount, e.NumberOfTurns) },
        { EffectType.AdjustPlayerStats, e => new PlayerEffect(p => p.AddState(
            new AdjustedPlayerStats(new PlayerStatAddends().With(e.EffectScope.Value.EnumVal<PlayerStatType>(), e.IntAmount), e.NumberOfTurns, e.IntAmount < 0))) },
        { EffectType.GainCredits, e => new PartyEffect(p => p.UpdateCreditsBy(e.TotalIntAmount)) },
        { EffectType.AtStartOfTurn, e => new StartOfTurnEffect(e) },
        { EffectType.AtEndOfTurn, e => new EndOfTurnEffect(e) },
        { EffectType.DelayedStartOfTurn, e => new DelayedStartOfTurnEffect(e) },
        { EffectType.EnterStealth, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(new AdjustedStats(new StatAddends().With(TemporalStatType.Stealth, 1), 
            TemporalStateMetadata.BuffForDuration(e.NumberOfTurns, new StatusDetail(StatusTag.Stealth))))) },
        { EffectType.GainDoubleDamage, e => new SimpleEffect(m => m.Adjust(TemporalStatType.DoubleDamage, e.IntAmount))},
        { EffectType.AntiHeal, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(
            new AdjustedStats(new StatMultipliers().With(StatType.Healability, 0.5f),  TemporalStateMetadata.DebuffForDuration(e.NumberOfTurns, new StatusDetail(StatusTag.AntiHeal)))))},
        { EffectType.FullyReviveAllAllies, e => new FullyReviveAllAllies() },
        { EffectType.SwapLifeForce, e => new SwapLifeForce() },
        { EffectType.DuplicateStatesOfType, e => new DuplicateStatesOfType(e.StatusTag)},
        { EffectType.DealRawDamageFormula, e => new FullContextEffect((ctx, m) => m.TakeRawDamage(Mathf.CeilToInt(Formula.Evaluate(ctx.SourceStateSnapshot, m, e.Formula, ctx.XPaidAmount))))},
        { EffectType.ApplyAdditiveStatInjury, e => new AegisPreventable(new ApplyStatInjury(StatOperation.Add, e.EffectScope, e.TotalAmount, e.FlavorText), "Injury") },
        { EffectType.ApplyMultiplicativeStatInjury, e => new AegisPreventable(new ApplyStatInjury(StatOperation.Multiply, e.EffectScope, e.TotalAmount, e.FlavorText), "Injury") },
        { EffectType.Kill, e => new SimpleEffect(m => m.SetHp(0)) },
        { EffectType.ShowCustomTooltip, e => new SimpleEffect(m => m.AddCustomStatus(
            new CustomStatusIcon(e.FlavorText, e.EffectScope, e.IntAmount, e.ForSimpleDurationStatAdjustment()))) },
        { EffectType.OnDeath, e => new EffectOnDeath(false, e.IntAmount, e.NumberOfTurns, e.ReactionSequence) },
        { EffectType.DoubleTheEffectAndMinusDurationTransformer, e => new EffectDoubleTheEffectAndMinus1Duration(e) },
        { EffectType.PlayBonusCardAfterNoCardPlayedInXTurns, e => new SimpleEffect(m => m.ApplyBonusCardPlayer(
            new PlayBonusCardAfterNoCardPlayedInXTurns(e.EffectScope, e.BonusCardType, e.TotalIntAmount, e.StatusDetail)))},
        { EffectType.HealFormula, e => new FullContextEffect((ctx, m) => m.GainHp(Formula.Evaluate(ctx.SourceStateSnapshot, m, e.Formula, ctx.XPaidAmount))) },
        { EffectType.AttackFormula, e => new Attack(new PhysicalDamage((ctx, m) => Formula.Evaluate(ctx.SourceStateSnapshot, m.State, e.Formula, ctx.XPaidAmount)), e.HitsRandomTargetMember)},
        { EffectType.MagicAttackFormula, e => new MagicAttack(new SpellDamage((ctx, m) => Formula.Evaluate(ctx.SourceStateSnapshot, m.State, e.Formula, ctx.XPaidAmount)), e.HitsRandomTargetMember)},
        { EffectType.AddToXCostTransformer, e => new EffectAddToXCostTransformer(e) },
        { EffectType.RedrawHandOfCards, e => new FullContextEffect(ctx => { BattleLog.Write("Discard and drew a new hand of cards."); 
            ctx.PlayerCardZones.DiscardHand(); ctx.PlayerCardZones.DrawCards(ctx.PlayerState.CardDraws - ctx.PlayerCardZones.PlayZone.Count); })},
        { EffectType.DrawCards, e => new FullContextEffect(ctx => ctx.PlayerCardZones.DrawCards(
            BattleLoggedItem(v => $"Drew {v} cards", Formula.Evaluate(ctx.SourceStateSnapshot, e.Formula, ctx.XPaidAmount).CeilingInt())))},
        { EffectType.GlitchRandomCards, e => new GlitchCards(e.BaseAmount, e.EffectScope, cards => cards) },
        { EffectType.LeaveBattle, e => new SimpleEffect(m => Message.Publish(new DespawnEnemy(m))) },
        { EffectType.ResetStatToBase, e => new SimpleEffect(m => m.ResetStatToBase(e.EffectScope))},
        { EffectType.TransferPrimaryResourceFormula, e => new TransferPrimaryResource((ctx, m) => 
            BattleLoggedItem(v => $"{m.Name} {GainedOrLostTerm(v)} {v} {m.State.PrimaryResource.Name}", 
                Formula.Evaluate(ctx.SourceSnapshot.State, m.State, e.Formula, ctx.XPaidAmount).CeilingInt())) },
        { EffectType.AdjustCardTagPrevention, e => new SimpleEffect(m => m.PreventCardTag(e.EffectScope.Value.EnumVal<CardTag>(), e.BaseAmount)) },
        { EffectType.Reload, e => new SimpleEffect(m => BattleLogged($"{m.Name} Reloaded", 
            () => m.AdjustPrimaryResource(99))) }
    };

    private static string GainedOrLostTerm(float amount) => amount > 0 ? "gained" : "lost";

    private static T BattleLoggedItem<T>(Func<T, string> createMessage, T value)
    {
        BattleLog.Write(createMessage(value));
        return value;
    }
    
    private static void BattleLogged(string msg, Action action)
    {
        action();
        BattleLog.Write(msg);
    }
    
    public static void Apply(EffectData effectData, EffectContext ctx)
    {
        try
        {
            if (ctx.Target.Members.Length == 0)
                return;
            
            effectData = ctx.Source.State.Transform(effectData, ctx);
            var effect = Create(effectData);
            var whenClause = effectData.TurnDelay == 0
                             ? "" 
                             : effectData.TurnDelay == 1
                                ? " at the start of next turn" 
                                : $" in {effectData.TurnDelay} turns";
            DevLog.Write($"Applying Effect of {effectData.EffectType} to {ctx.Target.MembersDescriptions()}{whenClause}");
            if (effectData.TurnDelay > 0)
                BattleLog.Write($"Will Apply {effectData.EffectType}{whenClause}");
            effect.Apply(ctx);
        }
        catch (Exception e)
        {
            Log.Error($"EffectType {effectData.EffectType} is broken {e}");
            #if UNITY_EDITOR
            throw;
            #endif
        }
    }

    public static Effect Create(EffectData effectData)
    {
        try
        {
            if (effectData.TurnDelay > 0)
                return Create(new EffectData
                {
                    EffectType = EffectType.DelayedStartOfTurn,
                    NumberOfTurns = new IntReference(effectData.TurnDelay),
                    ReferencedSequence = AsCardActionsData(effectData.Immediately()),
                    StatusTag = StatusTag.StartOfTurnTrigger
                });
            
            var effectType = effectData.EffectType;
            if (!CreateEffectOfType.ContainsKey(effectData.EffectType))
            {
                Log.Error($"No EffectType of {effectData.EffectType} exists in {nameof(AllEffects)}");
                return CreateEffectOfType[EffectType.Nothing](effectData);
            }

            return CreateEffectOfType[effectType](effectData);
        }
        catch (Exception e)
        {
            Log.Error($"EffectType {effectData.EffectType} is broken {e}");
            return CreateEffectOfType[EffectType.Nothing](effectData);
        }
    }
    
    private static CardActionsData AsCardActionsData(EffectData effectData)
        => ((CardActionsData)FormatterServices.GetUninitializedObject(typeof(CardActionsData)))
            .Initialized(new CardActionV2(effectData));
    
    private static int RoundUp(float v) => Mathf.CeilToInt(v);
}
