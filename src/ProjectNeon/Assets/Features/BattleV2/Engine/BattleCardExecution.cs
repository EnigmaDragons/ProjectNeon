using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleCardExecution
{
    // Card
    public static void Play(this Card card, Target[] targets, ResourceQuantity xAmountPaid, BattleStateSnapshot battleStateSnapshot)
    {
        if (card.ActionSequences.Length > targets.Length)
            Log.Error($"{card.Name}: For {card.ActionSequences.Length} there are only {targets.Length} targets");

        for (var i = 0; i < card.ActionSequences.Length; i++)
        {
            var seq = card.ActionSequences[i];
            var selectedTarget = targets[i];
            var avoidingMembers = GetAvoidingMembers(seq, selectedTarget);
            var ctx = new CardActionContext(card.Owner, selectedTarget, avoidingMembers, seq.Group, seq.Scope, xAmountPaid, battleStateSnapshot);
            ResolveSequenceAsync(seq, ctx);
        }
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
    
    public static IPayloadProvider Play(this CardActionsData card, StatusEffectContext ctx)
        => new MultiplePayloads(card.Actions.Select(x => x.Play(ctx)));
    
    public static IPayloadProvider PlayAsReaction(this CardActionsData card, Member source, Target target, int amountPaid)
        => new MultiplePayloads(card.Actions.Select(x => x.PlayReaction(source, target, amountPaid)).ToArray());

    // Individual Actions
    private static IPayloadProvider Play(this CardActionV2 action, StatusEffectContext ctx)
        => Play(action, new CardActionContext(ctx.Source, new Single(ctx.Member), 
            Array.Empty<Member>(), Group.Self, Scope.One, new ResourceQuantity(), new BattleStateSnapshot()));
    
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
                    new SinglePayload(new ApplyBattleEffect(action.BattleEffect, ctx.Source, effectedTargets, ctx.Group, ctx.Scope, isReaction: false)), 
                    new SinglePayload(new CardActionAvoided(action.BattleEffect, ctx.Source, effectedTargets, ctx.AvoidingMembers)))
                : (IPayloadProvider)new SinglePayload(new ApplyBattleEffect(action.BattleEffect, ctx.Source, effectedTargets, ctx.Group, ctx.Scope, isReaction: false));
        if (type == CardBattleActionType.SpawnEnemy)
            return new SinglePayload(new SpawnEnemy(action.EnemyToSpawn));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(new CharacterAnimationRequested(ctx.Source.Id, action.CharacterAnimation, ctx.Target));
        if (type == CardBattleActionType.AnimateAtTarget)
            return new SinglePayload(new BattleEffectAnimationRequested { EffectName = action.AtTargetAnimation, PerformerId = ctx.Source.Id, Target = ctx.Target, Scope = ctx.Scope, Group = ctx.Group });
        if (type == CardBattleActionType.Condition)
            return new DelayedPayload(() => AllConditions.Resolve(action.ConditionData, ctx));
        throw new Exception($"Unrecognized card battle action type: {type}");
    }
    
    private static IPayloadProvider PlayReaction(this CardActionV2 action, Member source, Target target, int amountPaid)
    {
        var type = action.Type;
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(action.BattleEffect, source, target));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(new CharacterAnimationRequested(source.Id, action.CharacterAnimation, target));
        //if (type == CardBattleActionType.AnimateAtTarget)
            // TODO: Implement Reactive Scope Animations
        //if (type == CardBattleActionType.Condition)
            // TODO: Implement Conditional Reactive Effects if needed (probably not needed)
        throw new Exception($"Unhandled card battle action type: {type}");
    }
}
