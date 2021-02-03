using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public static class AllEffects
{
    private static readonly Dictionary<EffectType, Func<EffectData, Effect>> CreateEffectOfType = new Dictionary<EffectType, Func<EffectData, Effect>>
    {
        { EffectType.Nothing, e => new NoEffect() },
        { EffectType.PhysicalDamage, e => new DealDamage(new PhysicalDamage(e.BaseAmount, e.FloatAmount)) },
        { EffectType.AdjustStatAdditivelyFormula, e => new FullContextEffect((ctx, m) => m.ApplyTemporaryAdditive(new AdjustedStats(
            BattleLoggedItem(s => $"Stats of {m.Name} are adjusted by {s}",
                new StatAddends().WithRaw(e.EffectScope, Formula.Evaluate(new FormulaContext(ctx.SourceSnapshot.State, m, ctx.XPaidAmount), e.Formula))), 
                e.ForSimpleDurationStatAdjustment())))},
        { EffectType.AdjustStatMultiplicatively, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(
            new AdjustedStats(new StatMultipliers().WithRaw(e.EffectScope, e.FloatAmount), e.ForSimpleDurationStatAdjustment())))},
        { EffectType.ReactWithEffect, e => new EffectReactWith(false, e.IntAmount, e.NumberOfTurns, e.StatusDetail, 
            ReactiveTriggerScopeExtensions.Parse(e.EffectScope), e.ReactionConditionType, e.ReactionEffect)},
        { EffectType.ReactWithCard, e => new EffectReactWith(false, e.IntAmount, e.NumberOfTurns, e.StatusDetail, 
            ReactiveTriggerScopeExtensions.Parse(e.EffectScope), e.ReactionConditionType, e.ReactionSequence)},
        { EffectType.RemoveDebuffs, e => new SimpleEffect(m => BattleLogged($"{m.Name} has been cleansed of all debuffs", m.CleanseDebuffs))},
        { EffectType.AdjustCounterFormula, e => new FullContextEffect((ctx, m) => m.Adjust(e.EffectScope, Formula.Evaluate(new FormulaContext(ctx.SourceSnapshot.State, m, ctx.XPaidAmount), e.Formula)))},
        { EffectType.ShieldFlat, e => new ShieldFlat(e.IntAmount) },
        { EffectType.ResourceFlat, e => new SimpleEffect(m => m.GainResource(e.EffectScope.Value, e.IntAmount))},
        { EffectType.DamageOverTimeFlat, e => new DamageOverTime(e) },
        { EffectType.DamageOverTime, e => new DamageOverTime(e) },
        { EffectType.ApplyVulnerable, e => new SimpleEffect(m => BattleLogged($"{m.Name} has become vulnerable",
            () => m.ApplyTemporaryMultiplier(new AdjustedStats(new StatMultipliers().With(StatType.Damagability, 1.33f), TemporalStateMetadata.DebuffForDuration(e.NumberOfTurns, new StatusDetail(StatusTag.Vulnerable)))))) },
        { EffectType.ShieldToughness, e => new SimpleEffect((src, m) => m.AdjustShield(e.FloatAmount * src.State.Toughness())) },
        { EffectType.RemoveShields, e => new SimpleEffect(m => BattleLogged($"{m.Name} lost all their shields", () => m.AdjustShield(-999))) },
        { EffectType.StunForTurns, e => new SimpleEffect(m => BattleLogged($"{m.Name} is stunned for {e.NumberOfTurns} turns.", () => m.ApplyTemporaryAdditive(new StunForTurns(e.NumberOfTurns))))},
        { EffectType.StunForNumberOfCards, e => new SimpleEffect(m => BattleLogged($"{m.Name} has been stunned for the next {e.IntAmount} cards.", 
            () => m.ApplyTemporaryAdditive(AdjustedStats.CreateIndefinite(new StatAddends().With(TemporalStatType.CardStun, e.IntAmount), true)))) },
        { EffectType.InterceptAttackForTurns, e => new InterceptAttack(e.NumberOfTurns)},
        { EffectType.Attack, e => new Attack(new PhysicalDamage(e.BaseAmount, e.FloatAmount), e.HitsRandomTargetMember)},
        { EffectType.HealOverTime, e => new HealOverTime(e.FloatAmount, e.NumberOfTurns) },
        { EffectType.ReactOnEvadedWithCard, e => new EffectOnAvoided(false, e.IntAmount, e.NumberOfTurns, e.StatusDetail, ReactiveTriggerScopeExtensions.Parse(e.EffectScope), AvoidanceType.Evade, e.ReactionSequence) },
        { EffectType.ReactOnSpellshieldedWithCard, e => new EffectOnAvoided(false, e.IntAmount, e.NumberOfTurns, e.StatusDetail, ReactiveTriggerScopeExtensions.Parse(e.EffectScope), AvoidanceType.Spellshield, e.ReactionSequence) },
        { EffectType.HealMagic, e => new Heal(e.BaseAmount, e.FloatAmount, StatType.Magic) },
        { EffectType.HealToughness, e => new Heal(e.BaseAmount, e.FloatAmount, StatType.Toughness) },
        { EffectType.AdjustPrimaryResource, e => new SimpleEffect(m => BattleLogged($"{m.Name} {GainedOrLostTerm(e.TotalIntAmount)} {Math.Abs(e.TotalIntAmount)} {m.PrimaryResource.Name}", 
            () => m.AdjustPrimaryResource(e.IntAmount + e.BaseAmount))) },
        { EffectType.AdjustPlayerStats, e => new PlayerEffect(p => p.AddState(
            new AdjustedPlayerStats(new PlayerStatAddends().With((PlayerStatType)Enum.Parse(typeof(PlayerStatType), e.EffectScope), e.IntAmount), e.NumberOfTurns, e.IntAmount < 0))) },
        { EffectType.MagicAttack, e => new MagicAttack(new SpellDamage(e.BaseAmount, e.FloatAmount), e.HitsRandomTargetMember) },
        { EffectType.GainCredits, e => new PartyEffect(p => p.UpdateCreditsBy(e.TotalIntAmount)) },
        { EffectType.AtStartOfTurn, e => new StartOfTurnEffect(e) },
        { EffectType.AtEndOfTurn, e => new EndOfTurnEffect(e) },
        { EffectType.DelayedStartOfTurn, e => new DelayedStartOfTurnEffect(e) },
        { EffectType.MagicDamageOverTime, e => new MagicDamageOverTime(e)},
        { EffectType.PhysicalDamageOverTime, e => new PhysicalDamageOverTime(e)},
        { EffectType.HealPercentMissingHealth, e => new SimpleEffect(m => m.GainHp(Mathf.CeilToInt(m.MissingHp() * e.FloatAmount))) },
        { EffectType.EnterStealth, e => new SimpleEffect(m => m.Adjust(TemporalStatType.Stealth, e.NumberOfTurns + 1)) },
        { EffectType.GainDoubleDamage, e => new SimpleEffect(m => m.Adjust(TemporalStatType.DoubleDamage, e.IntAmount))},
        { EffectType.AntiHeal, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(
            new AdjustedStats(new StatMultipliers().With(StatType.Healability, 0.5f),  TemporalStateMetadata.DebuffForDuration(e.NumberOfTurns, new StatusDetail(StatusTag.AntiHeal)))))},
        { EffectType.FullyReviveAllAllies, e => new FullyReviveAllAllies() },
        { EffectType.ApplyConfusion, e => new SimpleEffect(m => m.Adjust(TemporalStatType.Confusion, e.NumberOfTurns + 1)) },
        { EffectType.SwapLifeForce, e => new SwapLifeForce() },
        { EffectType.DuplicateStatesOfType, e => new DuplicateStatesOfType(e.StatusTag)},
        { EffectType.AdjustCounter, e => new SimpleEffect(m => m.Adjust(e.EffectScope, e.BaseAmount))},
        { EffectType.ShieldToughnessBasedOnNumberOfOpponentDoTs, e => new ShieldToughnessBasedOnNumberOfOpponentDoTs(e.FloatAmount) },
        { EffectType.DealRawDamageFormula, e => new FullContextEffect((ctx, m) => m.TakeRawDamage(Mathf.CeilToInt(Formula.Evaluate(ctx.Source, e.Formula, ctx.XPaidAmount))))},
        { EffectType.ApplyAdditiveStatInjury, e => new ApplyStatInjury(StatOperation.Add, e.EffectScope, e.BaseAmount + e.FloatAmount, e.FlavorText)},
        { EffectType.ApplyMultiplicativeStatInjury, e => new ApplyStatInjury(StatOperation.Multiply, e.EffectScope, e.BaseAmount + e.FloatAmount, e.FlavorText)},
        { EffectType.Kill, e => new SimpleEffect(m => m.SetHp(0)) },
        { EffectType.ShowCustomTooltip, e => new SimpleEffect(m => m.AddCustomStatus(
            new CustomStatusIcon(e.FlavorText, e.EffectScope, e.IntAmount, e.ForSimpleDurationStatAdjustment()))) },
        { EffectType.OnDeath, e => new EffectOnDeath(false, e.IntAmount, e.NumberOfTurns, e.ReactionSequence) },
        { EffectType.Suicide, e => new SimpleEffect(m => m.SetHp(0)) },
        { EffectType.DoubleTheEffectAndMinusDurationTransformer, e => new EffectDoubleTheEffectAndMinus1Duration(e) },
        { EffectType.PlayBonusCardAfterNoCardPlayedInXTurns, e => new SimpleEffect(m => m.ApplyBonusCardPlayer(
            new PlayBonusCardAfterNoCardPlayedInXTurns(e.EffectScope, e.BonusCardType, e.TotalIntAmount, e.StatusDetail)))},
        { EffectType.HealFormula, e => new FullContextEffect((ctx, m) => m.GainHp(Formula.Evaluate(ctx.Source, e.Formula, ctx.XPaidAmount))) },
        { EffectType.AttackFormula, e => new Attack(new PhysicalDamage((ctx, m) => Formula.Evaluate(ctx.Source, e.Formula, ctx.XPaidAmount)), e.HitsRandomTargetMember)},
        { EffectType.MagicAttackFormula, e => new MagicAttack(new SpellDamage((ctx, m) => Formula.Evaluate(ctx.Source, e.Formula, ctx.XPaidAmount)), e.HitsRandomTargetMember)},
        { EffectType.AddToXCostTransformer, e => new EffectAddToXCostTransformer(e) },
        { EffectType.RedrawHandOfCards, e => new FullContextEffect(ctx => { BattleLog.Write("Discard and drew a new hand of cards."); 
            ctx.PlayerCardZones.DiscardHand(); ctx.PlayerCardZones.DrawHand(ctx.PlayerState.CardDraws - ctx.PlayerCardZones.PlayZone.Count); })}
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
}
