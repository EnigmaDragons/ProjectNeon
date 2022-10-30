using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

public static class AllEffects
{
    public static readonly bool Init = true;
    
    private static readonly Dictionary<EffectType, Func<EffectData, Effect>> CreateEffectOfType = new Dictionary<EffectType, Func<EffectData, Effect>>
    {
        { EffectType.Nothing, e => new NoEffect() },
        { EffectType.AdjustStatAdditivelyFormula, e => new AdjustStatsFormula(e, multiplicative: false)},
        { EffectType.AdjustStatMultiplicativelyFormula, e => new AdjustStatsFormula(e, multiplicative: true)},
        { EffectType.ReactWithEffect, e => new EffectReactWith(false, e.IntAmount, e.DurationFormula, e.StatusDetail, e.ReactionEffectScope, e.FinalReactionTimingWindow,
            ReactiveTriggerScopeExtensions.Parse(e.EffectScope), e.ReactionConditionType, e.ReactionEffect)},
        { EffectType.ReactWithCard, e => new EffectReactWith(false, e.IntAmount, e.DurationFormula, e.StatusDetail, e.ReactionEffectScope, e.FinalReactionTimingWindow,
            ReactiveTriggerScopeExtensions.Parse(e.EffectScope), e.ReactionConditionType, e.ReactionSequence)},
        { EffectType.RemoveDebuffs, e => new SimpleEffect(m => BattleLoggedItemIf(n => $"{m.NameTerm.ToEnglish()} has been cleansed of {n} debuffs", n => n > 0, m.CleanseDebuffs))},
        { EffectType.AdjustCounterFormula, e => new AdjustCounterFormula(e)},
        { EffectType.ShieldFormula, e => new AegisIfFormulaResult((ctx, amount, m) 
            => BattleLoggedItem(diff => $"{m.NameTerm.ToEnglish()} {GainedOrLostTerm(diff)} {diff} Shield", m.AdjustShield(amount)), e.Formula, amount => amount < 0)},
        { EffectType.ShieldRemoveAll, e => new AegisPreventable(new SimpleEffect(m => BattleLogged($"{m.NameTerm.ToEnglish()} lost all their shields", () => m.AdjustShield(-999))), "Losing All Shields") },
        { EffectType.ShieldBasedOnNumberOfOpponentsDoTs, e => new ShieldBasedOnNumberOfOpponentsDoTs(e.FloatAmount) },
        { EffectType.AdjustResourceFlat, e =>  AegisPreventable.If(
            new FullContextEffect((ctx, duration, m) => m.ChangeResource(new ResourceQuantity{ Amount = e.TotalIntAmount, ResourceType = e.EffectScope.Value }, ctx.AdventureState), 
                e.DurationFormula), e.TotalIntAmount < 0, "Resource Loss") },
        { EffectType.AdjustPrimaryResourceFormula, e => new AegisIfFormulaResult((ctx, amount, m) => 
            m.AdjustPrimaryResource(amount.CeilingInt()), e.Formula, amount => amount < 0) },
        { EffectType.DamageOverTimeFormula, e => new AegisPreventable(new DamageOverTimeFormula(e), "Damage Over Time") },
        { EffectType.DisableForTurns, e => new AegisPreventable(new FullContextEffect((ctx, duration, m) => BattleLogged($"{m.NameTerm.ToEnglish()} is disabled for {duration} turns.", 
            () => m.ApplyTemporaryAdditive(new DisableForTurns(ctx.Source.Id, duration))), e.DurationFormula), "Disable")},
        { EffectType.HealOverTime, e => new HealOverTime(e.FloatAmount, e.DurationFormula) },
        { EffectType.AdjustPlayerStats, e => new PlayerEffect((p, duration, amount) => p.AddState(
            new AdjustedPlayerStats(new PlayerStatAddends().With(e.EffectScope.Value.EnumVal<PlayerStatType>(), amount), duration, amount < 0)), e.DurationFormula, e.IntAmount.ToString()) },
        { EffectType.AdjustPlayerStatsFormula, e => new PlayerEffect((p, duration, amount) => p.AddState(
            new AdjustedPlayerStats(new PlayerStatAddends().With(e.EffectScope.Value.EnumVal<PlayerStatType>(), amount), duration, amount < 0)), e.DurationFormula, e.Formula) },
        { EffectType.GainCredits, e => new PartyEffect(p => BattleLoggedItem(v => $"Party {GainedOrLostTerm(v)} {v} Credits", p.UpdateCreditsBy(e.TotalIntAmount))) },
        { EffectType.AtStartOfTurn, e => new StartOfTurnEffect(e) },
        { EffectType.AtEndOfTurn, e => new EndOfTurnEffect(e) },
        { EffectType.DelayedStartOfTurn, e => new DelayedStartOfTurnEffect(e) },
        { EffectType.EnterStealth, e => new FullContextEffect((ctx, duration, m) => BattleLogged($"{m.NameTerm.ToEnglish()} entered Stealth", () => m.ApplyTemporaryAdditive(new AdjustedStats(new StatAddends().With(TemporalStatType.Stealth, 1), 
            TemporalStateMetadata.BuffForDuration(ctx.Source.Id, duration, new StatusDetail(StatusTag.Stealth))))), e.DurationFormula) },
        { EffectType.GainDoubleDamage, e => new SimpleEffect(m => m.Adjust(TemporalStatType.DoubleDamage, e.IntAmount))},
        { EffectType.FullyReviveAllAllies, e => new FullyReviveAllAllies() },
        { EffectType.SwapLifeForce, e => new SwapLifeForce() },
        { EffectType.DuplicateStatesOfType, e => new DuplicateStatesOfType(e.StatusTag)},
        { EffectType.DuplicateStatesOfTypeToRandomEnemy, e => new DuplicateStatesOfTypeToRandomEnemy(e.StatusTag)},
        { EffectType.DealTrueDamageFormula, e => new FullContextEffect((ctx, _, m) => 
            BattleLoggedItem(amount => $"{amount} true damage dealt to {m.NameTerm.ToEnglish()}", 
                m.TakeTrueDamage(Formula.EvaluateToInt(ctx.SourceStateSnapshot, m, e.Formula, ctx.XPaidAmount))), e.DurationFormula)},
        { EffectType.ApplyAdditiveStatInjury, e => new AegisPreventable(new ApplyStatInjury(StatOperation.Add, e.EffectScope, e.TotalAmount, e.FlavorText), "Injury") },
        { EffectType.ApplyMultiplicativeStatInjury, e => new AegisPreventable(new ApplyStatInjury(StatOperation.Multiply, e.EffectScope, e.TotalAmount, e.FlavorText), "Injury") },
        { EffectType.Kill, e => new SimpleEffect(m => m.SetHp(0)) },
        { EffectType.ShowCustomTooltip, e => new FullContextEffect((ctx, duration, m) => m.AddCustomStatus(
            new CustomStatusIcon(string.IsNullOrWhiteSpace(e.StatusDetailText) ? e.FlavorText: e.StatusDetailText, 
                e.StatusTag == StatusTag.None ? e.EffectScope : e.StatusTag.ToString(), e.IntAmount, 
                e.ForSimpleDurationStatAdjustment(ctx.Source.Id, duration))), e.DurationFormula) },
        { EffectType.OnDeath, e => new EffectOnDeath(false, e.IntAmount, e.DurationFormula, e.ReactionSequence, e.ReactionTimingWindow, e) },
        { EffectType.PlayBonusCardAfterNoCardPlayedInXTurns, e => new SimpleEffect(m => m.ApplyBonusCardPlayer(
            new PlayBonusCardAfterNoCardPlayedInXTurns(m.MemberId, e.BonusCardType, e.TotalIntAmount, e.StatusDetail)))},
        { EffectType.PlayBonusChainCard, e => new SimpleEffect(m => m.ApplyBonusCardPlayer(new PlayBonusChainCard(m.MemberId, e.BonusCardType, e.StatusDetail)))},
        { EffectType.HealFormula, e => new FullContextEffect((ctx, _, m) => m.GainHp(Formula.EvaluateToInt(ctx.SourceStateSnapshot, m, e.Formula, ctx.XPaidAmount)), e.DurationFormula) },
        { EffectType.AttackFormula, e => new Attack(new PhysicalDamage((ctx, m) => Formula.EvaluateToInt(ctx.SourceStateSnapshot, m.State, e.Formula, ctx.XPaidAmount)), e.HitsRandomTargetMember)},
        { EffectType.MagicAttackFormula, e => new MagicAttack(new SpellDamage((ctx, m) => Formula.EvaluateToInt(ctx.SourceStateSnapshot, m.State, e.Formula, ctx.XPaidAmount)), e.HitsRandomTargetMember)},
        { EffectType.TrueDamageAttackFormula, e => new TrueDamageAttack(new TrueDamageCalculation((ctx, m) => Formula.EvaluateToInt(ctx.SourceStateSnapshot, m.State, e.Formula, ctx.XPaidAmount)), e.HitsRandomTargetMember)},
        { EffectType.AddToXCostTransformer, e => new EffectAddToXCostTransformer(e) },
        { EffectType.CycleAllCardsInHand, e => new FullContextEffect((ctx, _) =>
            {
                BattleLog.Write("Cycle all cards in hand."); 
                ctx.PlayerState.AddFreeCycleCount(ctx.PlayerCardZones.HandZone.Count); 
                ctx.PlayerCardZones.CycleHand();
            }, e.DurationFormula)},
        { EffectType.DrawCards, e => new FullContextEffect((ctx, _) => ctx.PlayerCardZones.DrawCards(
            BattleLoggedItem(v => $"Drew {v} cards", Formula.EvaluateToInt(ctx.SourceStateSnapshot, e.Formula, ctx.XPaidAmount))), e.DurationFormula)},
        { EffectType.DrawCardsOfOwner, e => new FullContextEffect((ctx, _, m) => ctx.PlayerCardZones.DrawCards(
            BattleLoggedItem(v => $"Drew {v} cards for {ctx.Source.NameTerm.ToEnglish()}", Formula.EvaluateToInt(ctx.SourceStateSnapshot, m, e.Formula, ctx.XPaidAmount)), 
                card => card.Owner.Id == ctx.Source.Id), e.DurationFormula)},
        { EffectType.DrawCardsOfArchetype, e => new FullContextEffect((ctx, _) => ctx.PlayerCardZones.DrawCards(BattleLoggedItem(v => $"Drew {v} {e.EffectScope} cards", 
                Formula.EvaluateToInt(ctx.SourceStateSnapshot, e.Formula, ctx.XPaidAmount)), 
                    card => card.Archetypes.Contains(e.EffectScope.Value)), e.DurationFormula)},
        { EffectType.GlitchRandomCards, e => new GlitchCards(e.BaseAmount, e.EffectScope) },
        { EffectType.LeaveBattle, e => new SimpleEffect((m, __) =>
        {
            Message.Publish(new DisplayCharacterWordRequested(m, CharacterReactionType.LeftBattle));
            Message.Publish(new DespawnEnemy(m, true));
        }) },
        { EffectType.ResetStatToBase, e => new SimpleEffect(m => m.ResetStatToBase(e.EffectScope))},
        { EffectType.TransferPrimaryResourceFormula, e => new TransferPrimaryResource((ctx, m) => 
            BattleLoggedItem(v => $"{m.NameTerm.ToEnglish()} {GainedOrLostTerm(v)} {v} {m.State.PrimaryResource.Name}", 
                Formula.EvaluateToInt(ctx.SourceSnapshot.State, m.State, e.Formula, ctx.XPaidAmount))) },
        { EffectType.Reload, e => new SimpleEffect(m => BattleLogged($"{m.NameTerm.ToEnglish()} Reloaded", () => m.AdjustPrimaryResource(99))) },
        { EffectType.ResolveInnerEffect, e => new ResolveInnerEffect(e.ReferencedSequence?.BattleEffects?.ToArray() ?? Array.Empty<EffectData>()) },
        { EffectType.AdjustCostOfAllCardsInHandAtEndOfTurn, e => new AdjustAllCardsCostUntilPlayed(e.BaseAmount) },
        { EffectType.AdjustPrimaryStatForEveryCardCycledAndInHand, e => new AdjustPrimaryStatForEveryCardCycledAndInHand(e) },
        { EffectType.FillHandWithOwnersCards, e => new FillHandWithOwnersCards() },
        { EffectType.ChooseAndDrawCard, e => new ChooseAndDrawSelectedCard(e.EffectScope) },
        { EffectType.ChooseCardToCreate, e => new ChooseCardToCreate(e.EffectScope, e.Formula) },
        { EffectType.ChooseAndDrawCardOfArchetype, e => new ChooseAndDrawCardOfArchetype(e.EffectScope) },
        { EffectType.ChooseBuyoutCardsOrDefault, e => new ChooseBuyoutCardOrDefaultToCreate(e.EffectScope) },
        { EffectType.BuyoutEnemyById, e => new BuyoutEnemyById(e.EffectScope) },
        { EffectType.AdjustBattleRewardFormula, e => new AdjustBattleReward(e.EffectScope, e.Formula)},
        { EffectType.TransformCardsIntoCard, e => new TransformOwnersCardsIntoCard(e.EffectScope)},
        { EffectType.AdjustCardCosts, e => new AdjustCardCosts(e.EffectScope, e.Formula)},
        { EffectType.Drain, e => new Transfer(e.EffectScope.Value.MaybeEnumVal<TemporalStatType>(), 
            (ctx, m) => BattleLoggedItem(v => $"{m.NameTerm.ToEnglish()} {GainedOrLostTerm(v)} {v} {e.EffectScope.Value}", 
                Formula.EvaluateToInt(ctx.SourceSnapshot.State, m.State, e.Formula, ctx.XPaidAmount))) },
        { EffectType.AdjustOwnersPrimaryResourceBasedOnTargetShieldSum, 
            e => new FullContextEffect((ctx, t) => ctx.Source.State.AdjustPrimaryResource((ctx.Target.TotalShields() 
                * Formula.EvaluateRaw(ctx.SourceSnapshot.State, ctx.Source.State, e.Formula, ctx.XPaidAmount)).CeilingInt()))},
        { EffectType.RemoveTemporalModsMatchingStatusTag, e => new SimpleEffect(m => m.RemoveTemporaryEffects(t => t.Status.Tag.ToString() == e.EffectScope.Value))},
        { EffectType.InvulnerableForTurns, e => new FullContextEffect((ctx, duration, m) => m.ApplyTemporaryMultiplier(new AdjustedStats(
            new StatMultipliers().With(StatType.Damagability, 0f),
            TemporalStateMetadata.BuffForDuration(ctx.Source.Id, duration, new StatusDetail(StatusTag.Invulnerable, Maybe<string>.Missing())))), e.DurationFormula)},
        { EffectType.RandomEffect, e => new ResolveInnerEffect(e.ReferencedSequences?.Random()?.BattleEffects?.ToArray() ?? Array.Empty<EffectData>()) },
    };

    private static string GainedOrLostTerm(float amount) => amount > 0 ? "gained" : "lost";

    private static int Logged(int value)
    {
        Log.Info(value.ToString());
        return value;
    }

    private static T BattleLoggedItem<T>(Func<T, string> createMessage, T value)
    {
        BattleLog.Write(createMessage(value));
        return value;
    }
    
    private static T BattleLoggedItemIf<T>(Func<T, string> createMessage, Func<T, bool> messageCondition, Func<T> getValue)
    {
        var value = getValue();
        if (messageCondition(value))
            BattleLog.Write(createMessage(value));
        return value;
    }
    
    private static void BattleLogged(string msg, Action action)
    {
        action();
        BattleLog.Write(msg);
    }

    public static ApplyEffectResult Apply(EffectData effectData, EffectContext ctx)
    {
        try
        {
            // No Targets
            if (ctx.Target.Members.Length == 0)
                return new ApplyEffectResult(false, ctx);

            // Retargeting and Splitting
            var updatedContext = AutoRetargeted(effectData, ctx);
            if (effectData.ApplyToEachMemberIndividually && updatedContext.Target.Members.Length > 1)
            {
                var withoutAutoRetargeting = effectData.WithoutAutoRetargeting();
                DevLog.Info("Splitting Effect Targets");
                var applied = false;
                foreach (var targetMember in updatedContext.Target.Members)
                    if (Apply(withoutAutoRetargeting, updatedContext.Retargeted(ctx.Source, new Single(targetMember))).WasApplied)
                        applied = true;
                return new ApplyEffectResult(applied, updatedContext);
            }
            
            // Transform Effect
            var updatedEffectData = ctx.Source.State.Transform(effectData, ctx);
            
            // Calculate Effect Timing
            var whenClause = effectData.TurnDelay == 0
                ? "" 
                : effectData.TurnDelay == 1
                    ? " at the start of next turn" 
                    : $" in {effectData.TurnDelay} turns";
            DevLog.Write($"Applying Effect of {effectData.EffectType} to {updatedContext.Target.MembersDescriptions()}{whenClause}");
            if (effectData.TurnDelay > 0)
                BattleLog.Write($"Will Apply {effectData.EffectType}{whenClause}");
            
            // Check Conditions
            var shouldNotApplyReason = effectData.Condition().GetShouldNotApplyReason(updatedContext);
            if (shouldNotApplyReason.IsPresent)
            {
                DevLog.Write($"Did not apply {effectData.EffectType} because {shouldNotApplyReason.Value}");
                return new ApplyEffectResult(false, updatedContext);
            }
            
            // Apply Effect
            var effect = Create(updatedEffectData);
            effect.Apply(updatedContext);
            return new ApplyEffectResult(true, updatedContext);
        }
        catch (Exception e)
        {
            Log.Error($"EffectType {effectData.EffectType} is broken {e}");
            #if UNITY_EDITOR
            throw;
            #endif
            return new ApplyEffectResult(true, ctx);
        }
    }
    
    public static EffectContext AutoRetargeted(EffectData effectData, EffectContext ctx)
    {
        var updateTarget = effectData.ReTargetScope switch
            {
                AutoReTargetScope.None => ctx.Target,
                AutoReTargetScope.Source => new Single(ctx.Source),
                AutoReTargetScope.Everyone => new Multiple(ctx.BattleMembers.Values.Where(m => m.IsConscious())),
                AutoReTargetScope.AllAllies => new Multiple(ctx.BattleMembers.Values.Where(m => m.IsConscious() && m.TeamType == ctx.Source.TeamType)),
                AutoReTargetScope.RandomAlly => new Single(ctx.BattleMembers.Values.Where(m => m.IsConscious() && m.TeamType == ctx.Source.TeamType).Random()),
                AutoReTargetScope.RandomAllyExceptSelf => new Multiple(ctx.BattleMembers.Values.Where(m => m.IsConscious() && m.TeamType == ctx.Source.TeamType && ctx.Source.Id != m.Id).ToArray().Shuffled().Take(1)),
                AutoReTargetScope.AllAlliesExcept => AllAlliesExcept(ctx),
                AutoReTargetScope.AllAlliesExceptSelf => new Multiple(ctx.BattleMembers.Values.Where(m => m.IsConscious() && m.TeamType == ctx.Source.TeamType && m.Id != ctx.Source.Id)),
                AutoReTargetScope.AllEnemies => new Multiple(ctx.BattleMembers.Values.Where(m => m.IsConscious() && m.TeamType != ctx.Source.TeamType)),
                AutoReTargetScope.RandomEnemy => RandomEnemy(ctx),
                AutoReTargetScope.RandomEnemyExceptTarget => RandomEnemyExcludingTarget(ctx),
                AutoReTargetScope.AllEnemiesExcept => AllEnemiesExcept(ctx)
            };
        
        return ctx.Retargeted(ctx.Source, updateTarget);
    }

    public static Effect Create(EffectData effectData)
    {
        try
        {
            if (effectData.TurnDelay > 0)
                return Create(new EffectData
                {
                    EffectType = EffectType.DelayedStartOfTurn,
                    DurationFormula = effectData.TurnDelay.ToString(),
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
            #if UNITY_EDITOR
            throw;
            #endif
            return CreateEffectOfType[EffectType.Nothing](effectData);
        }
    }
    
    private static CardActionsData AsCardActionsData(EffectData effectData)
        => ((CardActionsData)FormatterServices.GetUninitializedObject(typeof(CardActionsData)))
            .Initialized(new CardActionV2(effectData));
    
    private static int RoundUp(float v) => Mathf.CeilToInt(v);
    
    private static Target AllAlliesExcept(EffectContext ctx)
    {
        var targets = ctx.BattleMembers.Values
            .Where(m => m.IsConscious() && m.TeamType == ctx.Source.TeamType)
            .ToArray()
            .Shuffled();
        return targets.AnyNonAlloc() ? new Multiple(targets.Take(targets.Length - 1)) : new Multiple(new Member[0]);
    }

    private static Target RandomEnemyExcludingTarget(EffectContext ctx) => RandomEnemy(ctx, ctx.Target.Members);
    private static Target RandomEnemy(EffectContext ctx) => RandomEnemy(ctx, new Member[0]);
    private static Target RandomEnemy(EffectContext ctx, Member[] excludedMembers)
    {
        var tauntEnemies = ctx.BattleMembers.Values
            .Where(m => m.IsConscious() && m.TeamType != ctx.Source.TeamType && m.HasTaunt() && !excludedMembers.Contains(m))
            .ToArray();
        if (tauntEnemies.AnyNonAlloc())
            return new Single(tauntEnemies.First());
        
        return new Multiple(ctx.BattleMembers.Values
            .Where(m => m.IsConscious() && m.TeamType != ctx.Source.TeamType && !excludedMembers.Contains(m))
            .ToArray()
            .Shuffled()
            .Take(1));
    }

    private static Target AllEnemiesExcept(EffectContext ctx)
    {
        var tauntEnemies = ctx.BattleMembers.Values
            .Where(m => m.IsConscious() && m.TeamType != ctx.Source.TeamType && m.HasTaunt())
            .ToArray()
            .Shuffled();
        if (tauntEnemies.AnyNonAlloc())
            return new Multiple(ctx.BattleMembers.Values.Where(m => m.IsConscious() && m.TeamType != ctx.Source.TeamType && m.Id != tauntEnemies[0].Id));
        var enemiesToIgnore = ctx.BattleMembers.Values
            .Where(m => m.IsConscious() && m.TeamType != ctx.Source.TeamType && !m.IsStealthed())
            .ToArray()
            .Shuffled();
        if (enemiesToIgnore.AnyNonAlloc())
            return new Multiple(ctx.BattleMembers.Values.Where(m => m.IsConscious() && m.TeamType != ctx.Source.TeamType && m.Id != enemiesToIgnore[0].Id));
        return new Multiple(new Member[0]);
    }
}
