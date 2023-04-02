using System;
using System.Collections.Generic;
using System.Linq;

public static class BattleCardExecution
{
    // Card
    public static void Play(this Card card, Target[] targets, BattleStateSnapshot battleStateSnapshot, ResourceQuantity xPaidAmount, ResourceQuantity paidAmount, Action onFinished)
    {
        QueuePayloads(card.Name, GetPayloads(card, targets, battleStateSnapshot, xPaidAmount, paidAmount), onFinished);
        card.OnPlayCard();
    }

    public static List<IPayloadProvider> GetPayloads(this Card card, Target[] targets, BattleStateSnapshot battleStateSnapshot, ResourceQuantity xPaidAmount, ResourceQuantity paidAmount)
    {
        if (card.ActionSequences.Length > targets.Length)
            Log.NonCrashingError($"{card.Name}: For {card.ActionSequences.Length} there are only {targets.Length} targets");

        var sequences = new List<CardActionSequence>();
        var sequenceTargets = new List<Target>();
        for (var i = 0; i < card.ActionSequences.Length && i < targets.Length; i++)
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

        var firstSequenceWithABattleEffectWithChosenTargetIndex = sequences.FirstIndexOf(x => x.CardActions.BattleEffects.Any() && x.Group == sequences[0].Group && x.Scope == sequences[0].Scope);
        for (var i = 0; i < sequences.Count; i++)
        {
            var seq = sequences[i];
            var selectedTarget = sequenceTargets[i];
            var ctx = new CardActionContext(card.Owner, selectedTarget, seq.Group, seq.Scope, xPaidAmount, paidAmount, battleStateSnapshot, card, new DoubleDamageContext(card.Owner, card.Owner.HasDoubleDamage()));
            var firstSequenceWithABattleEffectWithChosenTarget = i == firstSequenceWithABattleEffectWithChosenTargetIndex;
            payloads.Add(seq.cardActions.Play(ctx, firstSequenceWithABattleEffectWithChosenTarget)); 
        }

        return payloads;
    }
    
    private static void QueuePayloads(string cardName, List<IPayloadProvider> payloads, Action onFinished)
    {
        if (payloads.Count > 1)
            MessageGroup.Start(new MultiplePayloads(cardName, payloads), onFinished);
        else if (payloads.Count == 1)
            MessageGroup.Start(payloads[0], onFinished);
        else
            MessageGroup.Start(new NoPayload(), onFinished);
    }

    // Sequence Actions
    public static IPayloadProvider Play(this CardActionsData cardData, CardActionContext ctx, bool isFirstSequenceWithABattleEffectWithChosenTarget = true)
    {
        var payloadProviders = cardData.Actions.Select((x, i) => x.Play(false, ctx));
        if (isFirstSequenceWithABattleEffectWithChosenTarget) //hacky way to ensure certain reactions happen inspite of conditions not met
            payloadProviders = new[] {new SinglePayload(new ApplyBattleEffect(true, new EffectData { EffectType = EffectType.Nothing }, ctx.Source, ctx.Target, ctx.Card, ctx.XAmountPaid, ctx.AmountPaid, ctx.Preventions, ctx.Group, ctx.Scope, isReaction: false, ReactionTimingWindow.FirstCause, ctx.DoubleDamage)) }.Concat(payloadProviders);
        return new MultiplePayloads(cardData.Name, payloadProviders, 
            new SinglePayload(new Finished<CardActionContext> {Message = ctx}).AsArray());
    }

    public static IPayloadProvider Play(this CardActionsData cardData, StatusEffectContext ctx)
    {
        var firstBattleEffectIndex = cardData.Actions.FirstIndexOf(x => x.Type == CardBattleActionType.Battle);
        return new MultiplePayloads(cardData.Name, cardData.Actions.Select((x, i) => x.Play(i == firstBattleEffectIndex, ctx)));
    }

    public static IPayloadProvider PlayAsReaction(this CardActionsData cardData, Member source, Target target, ResourceQuantity xAmountPaid, ResourceQuantity amountPaid, string reactionName, ReactionTimingWindow timing)
    {
        var firstBattleEffectIndex = cardData.Actions.FirstIndexOf(x => x.Type == CardBattleActionType.Battle);
        return new MultiplePayloads(cardData.Name, cardData.Actions.Select((x, i) => x.PlayReaction(i == firstBattleEffectIndex, source,  target, xAmountPaid, amountPaid, timing))
            .ToArray());
    }

    // Individual Actions
    private static IPayloadProvider Play(this CardActionV2 action, bool isFirstBattleEffectWithChosenTarget, StatusEffectContext ctx)
        => Play(action, isFirstBattleEffectWithChosenTarget, new CardActionContext(ctx.Source, new Single(ctx.Member), 
            Group.Self, Scope.One, ResourceQuantity.None, ResourceQuantity.None, new BattleStateSnapshot(), Maybe<Card>.Missing(), new DoubleDamageContext(ctx.Source, false)));
    
    private static IPayloadProvider Play(this CardActionV2 action, bool isFirstBattleEffectOfChosenTarget, CardActionContext ctx)
    {
        var type = action.Type;
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(isFirstBattleEffectOfChosenTarget, action.BattleEffect, ctx.Source, ctx.Target, ctx.Card, ctx.XAmountPaid, ctx.AmountPaid, ctx.Preventions, ctx.Group, ctx.Scope, isReaction: false, ReactionTimingWindow.FirstCause, ctx.DoubleDamage));
        if (type == CardBattleActionType.SpawnEnemy)
            return new SinglePayload(new SpawnEnemy(action.EnemyToSpawn, action.EnemySpawnOffset, ctx.Source, action.Replacing, ctx.Card, ctx.XAmountPaid, ctx.AmountPaid, false, ReactionTimingWindow.Default, new Maybe<EffectCondition>(action.EnemySpawnCondition)));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(PayloadData.ExactMatch(new CharacterAnimationRequested2(ctx.Source.Id, action.CharacterAnimation2.Type)
            {
                Condition = new Maybe<EffectCondition>(action.CharacterAnimation2.Condition),
                Source = ctx.Source,
                Target = ctx.Target, 
                Card = ctx.Card,
                XPaidAmount = ctx.XAmountPaid,
                PaidAmount = ctx.AmountPaid
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
                SkipWaitingForCompletion = action.AtTargetAnimation.SkipWaitingForCompletion,
                Condition = action.AtTargetAnimation.Condition,
                Source = ctx.Source,
                Card = ctx.Card,
                XPaidAmount = ctx.XAmountPaid,
                PaidAmount = ctx.AmountPaid
            });
        if (type == CardBattleActionType.Condition)
            return new DelayedPayload(() => AllConditions.Resolve(action.ConditionData, ctx));
        throw new Exception($"Unrecognized card battle action type: {type}");
    }
    
    private static IPayloadProvider PlayReaction(this CardActionV2 action, bool isFirstBattleEffect, Member source, Target target, ResourceQuantity xAmountPaid, ResourceQuantity amountPaid, ReactionTimingWindow timing)
    {
        var type = action.Type;
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(isFirstBattleEffect, action.BattleEffect, source, target, Maybe<Card>.Missing(), xAmountPaid, amountPaid,
                new PreventionContextMut(target), isReaction: true, timing, new DoubleDamageContext(source, false)));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(PayloadData.ExactMatch(new CharacterAnimationRequested2(source.Id, action.CharacterAnimation2.Type)
            {
                Condition = new Maybe<EffectCondition>(action.CharacterAnimation2.Condition),
                Source = source,
                Target = target, 
                Card = Maybe<Card>.Missing(),
                XPaidAmount = xAmountPaid,
                PaidAmount = amountPaid
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
                SkipWaitingForCompletion = action.AtTargetAnimation.SkipWaitingForCompletion,
                Condition = action.AtTargetAnimation.Condition,
                Source = source,
                Card = Maybe<Card>.Missing(),
                XPaidAmount = xAmountPaid,
                PaidAmount = amountPaid
            });
        if (type == CardBattleActionType.SpawnEnemy)
            return new SinglePayload(new SpawnEnemy(action.EnemyToSpawn, action.EnemySpawnOffset, source, action.Replacing, Maybe<Card>.Missing(), xAmountPaid, amountPaid, true, timing, new Maybe<EffectCondition>(action.EnemySpawnCondition)));
        //if (type == CardBattleActionType.Condition)
            // TODO: Implement Conditional Reactive Effects if needed (probably not needed)
        throw new Exception($"Unhandled card battle action type: {type}");
    }
}
