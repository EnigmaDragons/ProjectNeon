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
            payloads.Add(new SinglePayload(new WaitDuringResolution(1f)));
        }

        var firstSequenceWithABattleEffectIndex = sequences.FirstIndexOf(x => x.CardActions.BattleEffects.Any());
        for (var i = 0; i < sequences.Count; i++)
        {
            var seq = sequences[i];
            var selectedTarget = sequenceTargets[i];
            var ctx = new CardActionContext(card.Owner, selectedTarget, seq.Group, seq.Scope, xPaidAmount, battleStateSnapshot, card);
            var isFirstSequenceWithABattleEffect = i == firstSequenceWithABattleEffectIndex;
            payloads.Add(seq.cardActions.Play(ctx, isFirstSequenceWithABattleEffect)); 
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
    public static IPayloadProvider Play(this CardActionsData cardData, CardActionContext ctx, bool isFirstSequenceWithABattleEffect = true)
    {
        var firstBattleEffectIndex = cardData.Actions.FirstIndexOf(x => x.Type == CardBattleActionType.Battle);
        return new MultiplePayloads(cardData.Actions.Select((x, i) => x.Play(isFirstSequenceWithABattleEffect && i == firstBattleEffectIndex, ctx)),
            new SinglePayload(new Finished<CardActionContext> {Message = ctx}).AsArray());
    }

    public static IPayloadProvider Play(this CardActionsData cardData, StatusEffectContext ctx)
    {
        var firstBattleEffectIndex = cardData.Actions.FirstIndexOf(x => x.Type == CardBattleActionType.Battle);
        return new MultiplePayloads(cardData.Actions.Select((x, i) => x.Play(i == firstBattleEffectIndex, ctx)));
    }

    public static IPayloadProvider PlayAsReaction(this CardActionsData cardData, Member source, Target target, ResourceQuantity xAmountPaid, string reactionName)
    {
        var firstBattleEffectIndex = cardData.Actions.FirstIndexOf(x => x.Type == CardBattleActionType.Battle);
        return new MultiplePayloads(cardData.Actions.Select((x, i) => x.PlayReaction(i == firstBattleEffectIndex, source,  target, xAmountPaid))
            .ToArray());
    }

    // Individual Actions
    private static IPayloadProvider Play(this CardActionV2 action, bool isFirstBattleEffect, StatusEffectContext ctx)
        => Play(action, isFirstBattleEffect, new CardActionContext(ctx.Source, new Single(ctx.Member), 
            Group.Self, Scope.One, ResourceQuantity.None, new BattleStateSnapshot(), Maybe<Card>.Missing()));
    
    private static IPayloadProvider Play(this CardActionV2 action, bool isFirstBattleEffect, CardActionContext ctx)
    {
        var type = action.Type;
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(isFirstBattleEffect, action.BattleEffect, ctx.Source, ctx.Target, ctx.Card, ctx.XAmountPaid, ctx.Preventions, ctx.Group, ctx.Scope, isReaction: false, ReactionTimingWindow.FirstCause));
        
        if (type == CardBattleActionType.SpawnEnemy)
            return new SinglePayload(new SpawnEnemy(action.EnemyToSpawn, action.EnemySpawnOffset));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(PayloadData.ExactMatch(new CharacterAnimationRequested2(ctx.Source.Id, action.CharacterAnimation2.Type)
            {
                Condition = new Maybe<EffectCondition>(action.CharacterAnimation2.Condition),
                Source = ctx.Source,
                Target = ctx.Target, 
                Card = ctx.Card,
                XPaidAmount = ctx.XAmountPaid
            }));
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
                Color = action.AtTargetAnimation.Color,
                Condition = action.AtTargetAnimation.Condition,
                Source = ctx.Source,
                Card = ctx.Card,
                XPaidAmount = ctx.XAmountPaid
            });
        if (type == CardBattleActionType.Condition)
            return new DelayedPayload(() => AllConditions.Resolve(action.ConditionData, ctx));
        throw new Exception($"Unrecognized card battle action type: {type}");
    }
    
    private static IPayloadProvider PlayReaction(this CardActionV2 action, bool isFirstBattleEffect, Member source, Target target, ResourceQuantity xAmountPaid)
    {
        var type = action.Type;
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(isFirstBattleEffect, action.BattleEffect, source, target, Maybe<Card>.Missing(), xAmountPaid, 
                new PreventionContextMut(target), isReaction: true, ReactionTimingWindow.ReactionCard));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(PayloadData.ExactMatch(new CharacterAnimationRequested2(source.Id, action.CharacterAnimation2.Type)
            {
                Condition = new Maybe<EffectCondition>(action.CharacterAnimation2.Condition),
                Source = source,
                Target = target, 
                Card = Maybe<Card>.Missing(),
                XPaidAmount = xAmountPaid
            }));
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
                Color = action.AtTargetAnimation.Color,
                Condition = action.AtTargetAnimation.Condition,
                Source = source,
                Card = Maybe<Card>.Missing(),
                XPaidAmount = xAmountPaid
            });
        if (type == CardBattleActionType.SpawnEnemy)
            return new SinglePayload(new SpawnEnemy(action.EnemyToSpawn, action.EnemySpawnOffset));
        //if (type == CardBattleActionType.Condition)
            // TODO: Implement Conditional Reactive Effects if needed (probably not needed)
        throw new Exception($"Unhandled card battle action type: {type}");
    }
}
