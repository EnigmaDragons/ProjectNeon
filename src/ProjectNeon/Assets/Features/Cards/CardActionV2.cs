using System;
using UnityEngine;

[Serializable]
public class CardActionV2
{
    [SerializeField] private CardBattleActionType type = CardBattleActionType.Battle;
    [SerializeField] private EffectData battleEffect = new EffectData();
    [SerializeField] private ActionConditionData conditionData = new ActionConditionData();
    [SerializeField] private StringReference characterAnimation;
    [SerializeField] private StringReference atTargetAnimation;

    public CardBattleActionType Type => type;
    public EffectData BattleEffect => battleEffect;
    public ActionConditionData ConditionData => conditionData;
    public StringReference CharacterAnimation => characterAnimation;
    public StringReference AtTargetAnimation => atTargetAnimation;
    
    public IPayloadProvider Play(Member source, Target target, Group group, Scope scope, int amountPaid)
    {
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(battleEffect, source, target));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(new CharacterAnimationRequested(source.Id, characterAnimation));
        if (type == CardBattleActionType.AnimateAtTarget)
            return new SinglePayload(new BattleEffectAnimationRequested { EffectName = atTargetAnimation, PerformerId = source.Id, Target = target, Scope = scope, Group = group });
        if (type == CardBattleActionType.Condition)
            return new DelayedPayload(() => AllConditions.Resolve(conditionData, source, target, group, scope, amountPaid));
        throw new Exception($"Unrecognized card battle action type: {Enum.GetName(typeof(CardBattleActionType), Type)}");
    }
    
    public IPayloadProvider Play(Member source, Target target, int amountPaid)
    {
        if (type == CardBattleActionType.Battle)
            return new SinglePayload(new ApplyBattleEffect(battleEffect, source, target));
        if (type == CardBattleActionType.AnimateCharacter)
            return new SinglePayload(new CharacterAnimationRequested(source.Id, characterAnimation));
        //if (type == CardBattleActionType.AnimateAtTarget)
            // TODO: Implement Reactive Scope Animations
        //if (type == CardBattleActionType.Condition)
            // TODO: Implement Conditional Reactive Effects if needed (probably not needed)
        throw new Exception($"Unrecognized card battle action type: {Enum.GetName(typeof(CardBattleActionType), Type)}");
    }
    
    public CardActionV2() {}
    public CardActionV2(EffectData e)
    {
        battleEffect = e;
    }
}
