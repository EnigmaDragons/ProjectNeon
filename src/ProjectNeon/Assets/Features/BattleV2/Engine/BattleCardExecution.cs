using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleCardExecution
{
    // Card
    public static void Play(this Card card, Target[] targets, BattleStateSnapshot battleStateSnapshot)
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
                sequences.AddRange(Enumerable.Range(0, card.LockedXValue.Value.Amount).Select(_ => seq));
                sequenceTargets.AddRange(Enumerable.Range(0, card.LockedXValue.Value.Amount).Select(_ => targets[i]));
            }
            else
            {
                sequences.Add(seq);
                sequenceTargets.Add(targets[i]);
            }
        }
        
        for (var i = 0; i < sequences.Count; i++)
        {
            var seq = sequences[i];
            var selectedTarget = sequenceTargets[i];
            var avoidingMembers = GetAvoidingMembers(seq, selectedTarget);
            var avoidanceWord = seq.AvoidanceType == AvoidanceType.Evade ? "Evaded" : "Spellshielded";
            if (avoidingMembers.Any())
                BattleLog.Write($"{string.Join(", ", avoidingMembers.Select(a => a.Name))} {avoidanceWord} {card.Name}");
            var ctx = new CardActionContext(card.Owner, selectedTarget, avoidingMembers, seq.Group, seq.Scope, card.LockedXValue.Value, battleStateSnapshot, card);
            ResolveSequenceAsync(seq, ctx);
        }
        SequenceMessage.Queue(new NoPayload());
    }

    private static Member[] GetAvoidingMembers(CardActionSequence seq, Target selectedTarget)
    {
        var avoidingMembers = new List<Member>();

        if (seq.AvoidanceType == AvoidanceType.Evade)
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

        if (seq.AvoidanceType == AvoidanceType.Spellshield)
        {
            foreach (var member in selectedTarget.Members.Where(m => m.State[TemporalStatType.Spellshield] > 0))
            {
                member.State.AdjustSpellshield(-1);
                avoidingMembers.Add(member);
            }
        }

        return avoidingMembers.ToArray();
    }

    private static void ResolveSequenceAsync(CardActionSequence seq, CardActionContext ctx)
    {
        SequenceMessage.Queue(seq.cardActions.Play(ctx));
    }
    
    // Sequence Actions
    public static IPayloadProvider Play(this CardActionsData card, CardActionContext ctx)
        => new MultiplePayloads(card.Actions.Select(x => x.Play(ctx)));
    
    public static IPayloadProvider Play(this CardActionsData cardData, StatusEffectContext ctx)
        => new MultiplePayloads(cardData.Actions.Select(x => x.Play(ctx)));
    
    public static IPayloadProvider PlayAsReaction(this CardActionsData cardData, Member source, Target target, ResourceQuantity xAmountPaid)
        => new MultiplePayloads(cardData.Actions.Select(x => x.PlayReaction(source, target, xAmountPaid)).ToArray());

    // Individual Actions
    private static IPayloadProvider Play(this CardActionV2 action, StatusEffectContext ctx)
        => Play(action, new CardActionContext(ctx.Source, new Single(ctx.Member), 
            Array.Empty<Member>(), Group.Self, Scope.One, ResourceQuantity.None, new BattleStateSnapshot(), Maybe<Card>.Missing()));
    
    private static IPayloadProvider Play(this CardActionV2 action, CardActionContext ctx)
    {
        var type = action.Type;
        var effectedTargets = new Multiple(ctx.Target.Members.Where(m => !ctx.AvoidingMembers.Any(am => am.Id == m.Id)).ToArray());
        var allAvoidedEffect = ctx.AvoidingMembers.Any() && effectedTargets.Members.Length == 0;
        if (type == CardBattleActionType.Battle && allAvoidedEffect)
            return new SinglePayload(new CardActionAvoided(action.BattleEffect, ctx.Source, effectedTargets, ctx.AvoidingMembers));
        if (type == CardBattleActionType.Battle)
            return ctx.AvoidingMembers.Any() 
                ? new MultiplePayloads(
                    new SinglePayload(new ApplyBattleEffect(action.BattleEffect, ctx.Source, effectedTargets, ctx.Card, ctx.XAmountPaid, ctx.Group, ctx.Scope, isReaction: false)), 
                    new SinglePayload(new CardActionAvoided(action.BattleEffect, ctx.Source, effectedTargets, ctx.AvoidingMembers)))
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
    
    private static IPayloadProvider PlayReaction(this CardActionV2 action, Member source, Target target, ResourceQuantity xAmountPaid)
    {
        var type = action.Type;
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
