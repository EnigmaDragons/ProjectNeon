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
    
    public void Play(Member source, Target target, Group group, Scope scope, int amountPaid)
    {
        if (type == CardBattleActionType.Battle)
            Message.Publish(new ApplyBattleEffect(battleEffect, source, target));
        else if (type == CardBattleActionType.AnimateCharacter)
            Message.Publish(new CharacterAnimationRequested(source.Id, characterAnimation));
        else if (type == CardBattleActionType.AnimateAtTarget)
            Message.Publish(new BattleEffectAnimationRequested { EffectName = atTargetAnimation, PerformerId = source.Id, Target = target, Scope = scope, Group = group });
    }
}