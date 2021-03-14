using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleCardExecution
{
    // Card
    public static void Play(this Card card, Target[] targets, BattleStateSnapshot battleStateSnapshot, ResourceQuantity xPaidAmount, Action onFinished)
    {
        if (card.ActionSequences.Length > targets.Length)
            Log.Error($"{card.Name}: For {card.ActionSequences.Length} there are only {targets.Length} targets");

        var sequences = new List<CardActionSequence>();
        var sequenceTargets = new List<Target>();
        for (var i = 0; i < card.ActionSequences.Length; i++)
        {
            var seq = card.ActionSequences[i];
            if (seq.RepeatX)
            {
                sequences.AddRange(Enumerable.Range(0, xPaidAmount.Amount).Select(_ => seq));
                sequenceTargets.AddRange(Enumerable.Range(0, xPaidAmount.Amount).Select(_ => targets[i]));
            }
            else
            {
                sequences.Add(seq);
                sequenceTargets.Add(targets[i]);
            }
        }

        var payloads = new List<IPayloadProvider>();
        for (var i = 0; i < sequences.Count; i++)
        {
            var seq = sequences[i];
            var selectedTarget = sequenceTargets[i];
            var avoidingMembers = GetAvoidingMembers(seq.AvoidanceType, selectedTarget);
            var avoidanceWord = seq.AvoidanceType == AvoidanceType.Evade ? "Evaded" : "Spellshielded";
            if (avoidingMembers.Any())
                BattleLog.Write($"{string.Join(", ", avoidingMembers.Select(a => a.Name))} {avoidanceWord} {card.Name}");
            var avoidanceContext = new AvoidanceContext(seq.AvoidanceType, avoidingMembers);
            var ctx = new CardActionContext(card.Owner, selectedTarget, avoidanceContext, seq.Group, seq.Scope, xPaidAmount, battleStateSnapshot, card);
            payloads.Add(seq.cardActions.Play(ctx));
        }
        QueuePayloads(payloads, onFinished);
    }

    private static Member[] GetAvoidingMembers(AvoidanceType avoidance, Target selectedTarget)
    {
        var avoidingMembers = new List<Member>();

        if (avoidance == AvoidanceType.Evade)
        {
            foreach (var member in selectedTarget.Members.Where(m => m.State[TemporalStatType.Evade] > 0))
            {
                Log.Info($"{member.Name} is Avoiding");
                member.State.AdjustEvade(-1);
                // TODO: This Visual shouldn't happen here, nor at this time
                Message.Publish(new DisplaySpriteEffect(SpriteEffectType.Evade, member));
                Message.Publish(new PlayRawBattleEffect("EvadedText", Vector3.zero));
                avoidingMembers.Add(member);
            }
        }

        if (avoidance == AvoidanceType.Spellshield)
        {
            foreach (var member in selectedTarget.Members.Where(m => m.State[TemporalStatType.Spellshield] > 0))
            {
                member.State.AdjustSpellshield(-1);
                avoidingMembers.Add(member);
            }
        }

        return avoidingMembers.ToArray();
    }
    
    private static void QueuePayloads(List<IPayloadProvider> payloads, Action onFinished)
    {
        if (payloads.Count > 1)
            MessageGroup.Start(new MultiplePayloads(payloads), onFinished);
        else if (payloads.Count == 1)
            MessageGroup.Start(payloads[0], onFinished);
        else
            MessageGroup.Start(new NoPayload(), onFinished);
    }

    // Sequence Actions
    public static IPayloadProvider Play(this CardActionsData card, CardActionContext ctx)
        => new MultiplePayloads(card.Actions.Select(x => x.Play(ctx)));
    
    public static IPayloadProvider Play(this CardActionsData cardData, StatusEffectContext ctx)
        => new MultiplePayloads(cardData.Actions.Select(x => x.Play(ctx)));
    
    public static IPayloadProvider PlayAsReaction(this CardActionsData cardData, Member source, Target target, ResourceQuantity xAmountPaid, string reactionName, AvoidanceType avoidance)
    {
        var avoidingMembers = GetAvoidingMembers(avoidance, target);
        var avoidanceWord = avoidance == AvoidanceType.Evade ? "Evaded" : "Spellshielded";
        if (avoidingMembers.Any())
            BattleLog.Write($"{string.Join(", ", avoidingMembers.Select(a => a.Name))} {avoidanceWord} {reactionName}");
        return new MultiplePayloads(cardData.Actions.Select(x => x.PlayReaction(source, target, xAmountPaid, new AvoidanceContext(avoidance, avoidingMembers)))
            .ToArray());
    }

    // Individual Actions
    private static IPayloadProvider Play(this CardActionV2 action, StatusEffectContext ctx)
        => Play(action, new CardActionContext(ctx.Source, new Single(ctx.Member), new AvoidanceContext(AvoidanceType.UnavoidableStatusEffect,
            Array.Empty<Member>()), Group.Self, Scope.One, ResourceQuantity.None, new BattleStateSnapshot(), Maybe<Card>.Missing()));
    
    private static IPayloadProvider Play(this CardActionV2 action, CardActionContext ctx)
    {
        var type = action.Type;
        var effectedTargets = new Multiple(ctx.Target.Members.Where(m => !ctx.Avoid.Members.Any(am => am.Id == m.Id)).ToArray());
        var allAvoidedEffect = ctx.Avoid.Members.Any() && effectedTargets.Members.Length == 0;
        if (type == CardBattleActionType.Battle && ctx.Avoid.Type == AvoidanceType.Evade && ctx.Source.State[TemporalStatType.Blind] > 0)
            return new SinglePayload(new CardActionPrevented(ctx.Source, ctx.Avoid, TemporalStatType.Blind));
        if (type == CardBattleActionType.Battle && ctx.Avoid.Type == AvoidanceType.Spellshield && ctx.Source.State[TemporalStatType.Inhibit] > 0)
            return new SinglePayload(new CardActionPrevented(ctx.Source, ctx.Avoid, TemporalStatType.Inhibit));
        if (type == CardBattleActionType.Battle && allAvoidedEffect)
            return new SinglePayload(new CardActionAvoided(action.BattleEffect, ctx.Source, effectedTargets, ctx.Avoid));
        if (type == CardBattleActionType.Battle)
            return ctx.Avoid.Members.Any() 
                ? new MultiplePayloads(
                    new SinglePayload(new ApplyBattleEffect(action.BattleEffect, ctx.Source, effectedTargets, ctx.Card, ctx.XAmountPaid, ctx.Group, ctx.Scope, isReaction: false)), 
                    new SinglePayload(new CardActionAvoided(action.BattleEffect, ctx.Source, effectedTargets, ctx.Avoid)))
                : (IPayloadProvider)new SinglePayload(new ApplyBattleEffect(action.BattleEffect, ctx.Source, effectedTargets, ctx.Card, ctx.XAmountPaid, ctx.Group, ctx.Scope, isReaction: false));
        
        if (type == CardBattleActionType.SpawnEnemy)
            return new SinglePayload(new SpawnEnemy(action.EnemyToSpawn));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(new CharacterAnimationRequested(ctx.Source.Id, action.CharacterAnimation, ctx.Target));
        if (type == CardBattleActionType.AnimateAtTarget)
            return new SinglePayload(new BattleEffectAnimationRequested
            {
                EffectName = action.AtTargetAnimation.Animation, 
                PerformerId = ctx.Source.Id, 
                Target = ctx.Target, 
                Scope = ctx.Scope, 
                Group = ctx.Group, 
                Speed = action.AtTargetAnimation.Speed, 
                Size = action.AtTargetAnimation.Size, 
                Color = action.AtTargetAnimation.Color
            });
        if (type == CardBattleActionType.Condition)
            return new DelayedPayload(() => AllConditions.Resolve(action.ConditionData, ctx));
        throw new Exception($"Unrecognized card battle action type: {type}");
    }
    
    private static IPayloadProvider PlayReaction(this CardActionV2 action, Member source, Target target, ResourceQuantity xAmountPaid, AvoidanceContext avoid)
    {
        var type = action.Type;
        var effectedTargets = new Multiple(target.Members.Where(m => !avoid.Members.Any(am => am.Id == m.Id)).ToArray());
        var allAvoidedEffect = avoid.Members.Any() && effectedTargets.Members.Length == 0;
        if (type == CardBattleActionType.Battle && avoid.Type == AvoidanceType.Evade && source.State[TemporalStatType.Blind] > 0)
            return new SinglePayload(new CardActionPrevented(source, avoid, TemporalStatType.Blind));
        if (type == CardBattleActionType.Battle && avoid.Type == AvoidanceType.Spellshield && source.State[TemporalStatType.Inhibit] > 0)
            return new SinglePayload(new CardActionPrevented(source, avoid, TemporalStatType.Inhibit));
        if (type == CardBattleActionType.Battle && allAvoidedEffect)
            return new SinglePayload(new CardActionAvoided(action.BattleEffect, source, effectedTargets, avoid));
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(action.BattleEffect, source, target, Maybe<Card>.Missing(), xAmountPaid));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(new CharacterAnimationRequested(source.Id, action.CharacterAnimation, target));
        if (type == CardBattleActionType.AnimateAtTarget)
            return new SinglePayload(new BattleEffectAnimationRequested
            {
                EffectName = action.AtTargetAnimation.Animation, 
                PerformerId = source.Id, 
                Target = target, 
                Scope = target.Members.Length == 1 ? Scope.One : Scope.All, 
                Group = target.Members.All(x => x.TeamType == source.TeamType) ? Group.Ally : target.Members.All(x => x.TeamType != source.TeamType) ? Group.Opponent : Group.All, 
                Speed = action.AtTargetAnimation.Speed, 
                Size = action.AtTargetAnimation.Size, 
                Color = action.AtTargetAnimation.Color
            });
        //if (type == CardBattleActionType.Condition)
            // TODO: Implement Conditional Reactive Effects if needed (probably not needed)
        throw new Exception($"Unhandled card battle action type: {type}");
    }
}
