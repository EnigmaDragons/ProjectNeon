using System;
using UnityEngine;

[Serializable]
public class CardActionV2
{
    [SerializeField] private CardBattleActionType type = CardBattleActionType.Battle;
    [SerializeField] private EffectData battleEffect = new EffectData();
    [SerializeField] private ActionConditionData conditionData = new ActionConditionData();
    [SerializeField] private AnimationData characterAnimation;
    [SerializeField] private StringReference atTargetAnimation;
    [SerializeField] private Enemy enemyToSpawn;

    public CardBattleActionType Type => type;
    public EffectData BattleEffect => battleEffect;
    public ActionConditionData ConditionData => conditionData;
    public AnimationData CharacterAnimation => characterAnimation;
    public StringReference AtTargetAnimation => atTargetAnimation;

    public IPayloadProvider Play(StatusEffectContext ctx)
        => Play(new CardActionContext(ctx.Source, new Single(ctx.Member), Group.Self, Scope.One, new ResourceQuantity(),
            new BattleStateSnapshot()));
    
    public IPayloadProvider Play(CardActionContext ctx)
    {
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(battleEffect, ctx.Source, ctx.Target, ctx.Group, ctx.Scope, isReaction: false));
        if (type == CardBattleActionType.SpawnEnemy)
            return new SinglePayload(new SpawnEnemy(enemyToSpawn));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(new CharacterAnimationRequested(ctx.Source.Id, characterAnimation, ctx.Target));
        if (type == CardBattleActionType.AnimateAtTarget)
            return new SinglePayload(new BattleEffectAnimationRequested { EffectName = atTargetAnimation, PerformerId = ctx.Source.Id, Target = ctx.Target, Scope = ctx.Scope, Group = ctx.Group });
        if (type == CardBattleActionType.Condition)
            return new DelayedPayload(() => AllConditions.Resolve(conditionData, ctx));
        throw new Exception($"Unrecognized card battle action type: {Enum.GetName(typeof(CardBattleActionType), Type)}");
    }
    
    public IPayloadProvider PlayReaction(Member source, Target target, int amountPaid)
    {
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(battleEffect, source, target));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(new CharacterAnimationRequested(source.Id, characterAnimation, target));
        //if (type == CardBattleActionType.AnimateAtTarget)
            // TODO: Implement Reactive Scope Animations
        //if (type == CardBattleActionType.Condition)
            // TODO: Implement Conditional Reactive Effects if needed (probably not needed)
        throw new Exception($"Unrecognized card battle action type: {Enum.GetName(typeof(CardBattleActionType), Type)}");
    }
    
//    public IPayloadProvider PlayAsStatusEffect(Member source, Target target)
//    {
//        if (type == CardBattleActionType.Battle)
//            return new SinglePayload(new ApplyBattleEffect(battleEffect, source, target));
//        if (type == CardBattleActionType.AnimateCharacter)
//            return new SinglePayload(new CharacterAnimationRequested(source.Id, characterAnimation));
//        //if (type == CardBattleActionType.AnimateAtTarget)
//        // TODO: Implement Reactive Scope Animations
//        if (type == CardBattleActionType.Condition)
//            return new DelayedPayload(() => AllConditions.Resolve(conditionData, new Card));
//        throw new Exception($"Unrecognized card battle action type: {Enum.GetName(typeof(CardBattleActionType), Type)}");
//    }
//    
    public CardActionV2() {}
    public CardActionV2(EffectData e)
    {
        battleEffect = e;
    }
}
