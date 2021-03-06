using System;
using System.Collections.Generic;
using System.Linq;

public static class BattleCardExecution
{
    // Card
    public static void Play(this Card card, Target[] targets, BattleStateSnapshot battleStateSnapshot, ResourceQuantity xPaidAmount, Action onFinished)
    {
        QueuePayloads(GetPayloads(card, targets, battleStateSnapshot, xPaidAmount), onFinished);
        card.OnPlayCard();
    }

    public static List<IPayloadProvider> GetPayloads(this Card card, Target[] targets, BattleStateSnapshot battleStateSnapshot, ResourceQuantity xPaidAmount)
    {
        if (card.ActionSequences.Length > targets.Length)
            Log.Error($"{card.Name}: For {card.ActionSequences.Length} there are only {targets.Length} targets");

        var sequences = new List<CardActionSequence>();
        var sequenceTargets = new List<Target>();
        for (var i = 0; i < card.ActionSequences.Length; i++)
        {
            var seq = card.ActionSequences[i];
            var numRepetitions = seq.RepeatX ? xPaidAmount.Amount : seq.RepeatCount > 0 ? seq.RepeatCount : 1;
            sequences.AddRange(Enumerable.Range(0, numRepetitions).Select(_ => seq));
            sequenceTargets.AddRange(Enumerable.Range(0, numRepetitions).Select(_ => targets[i]));
        }

        var payloads = new List<IPayloadProvider>();
        var cardHasAnimations = card.ActionSequences.Sum(x => x.CardActions.NumAnimations) > 0;
        if (!cardHasAnimations)
        {
            Log.Warn($"{card.Name} has no animations. Adding wait time where animation would be.");
            payloads.Add(new SinglePayload(new WaitDuringResolution(1.2f)));
        }
        for (var i = 0; i < sequences.Count; i++)
        {
            var seq = sequences[i];
            var selectedTarget = sequenceTargets[i];
            var ctx = new CardActionContext(card.Owner, selectedTarget, seq.Group, seq.Scope, xPaidAmount, battleStateSnapshot, card);
            payloads.Add(seq.cardActions.Play(ctx));
        }

        return payloads;
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
        => new MultiplePayloads(card.Actions.Select(x => x.Play(ctx)), 
            new SinglePayload(new Finished<CardActionContext> { Message = ctx }).AsArray());
    
    public static IPayloadProvider Play(this CardActionsData cardData, StatusEffectContext ctx)
        => new MultiplePayloads(cardData.Actions.Select(x => x.Play(ctx)));
    
    public static IPayloadProvider PlayAsReaction(this CardActionsData cardData, Member source, Target target, ResourceQuantity xAmountPaid, string reactionName)
    {
        return new MultiplePayloads(cardData.Actions.Select(x => x.PlayReaction(source, target, xAmountPaid))
            .ToArray());
    }

    // Individual Actions
    private static IPayloadProvider Play(this CardActionV2 action, StatusEffectContext ctx)
        => Play(action, new CardActionContext(ctx.Source, new Single(ctx.Member), 
            Group.Self, Scope.One, ResourceQuantity.None, new BattleStateSnapshot(), Maybe<Card>.Missing()));
    
    private static IPayloadProvider Play(this CardActionV2 action, CardActionContext ctx)
    {
        var type = action.Type;
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(action.BattleEffect, ctx.Source, ctx.Target, ctx.Card, ctx.XAmountPaid, ctx.Preventions, ctx.Group, ctx.Scope, isReaction: false));
        
        if (type == CardBattleActionType.SpawnEnemy)
            return new SinglePayload(new SpawnEnemy(action.EnemyToSpawn, action.EnemySpawnOffset));
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
            return new SinglePayload(new ApplyBattleEffect(action.BattleEffect, source, target, Maybe<Card>.Missing(), xAmountPaid, new PreventionContextMut(target)));
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
