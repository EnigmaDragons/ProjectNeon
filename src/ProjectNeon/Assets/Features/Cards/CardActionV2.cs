using System;
using UnityEngine;

[Serializable]
public class CardActionV2
{
    [SerializeField] private CardBattleActionType type;
    [SerializeField] private EffectData battleEffect;
    [SerializeField] private StringReference characterAnimation;
    [SerializeField] private StringReference atTargetAnimation;

    public void Resolve(Member source, Target target, Group group, Scope scope)
    {
        if (type == CardBattleActionType.Battle)
            Message.Publish(new ApplyBattleEffect(battleEffect, source, target));
        else if (type == CardBattleActionType.AnimateCharacter)
            Message.Publish(new CharacterAnimationRequested(source.Id, characterAnimation));
        else if (type == CardBattleActionType.AnimateAtTarget)
            Message.Publish(new BattleEffectAnimationRequested { EffectName = atTargetAnimation, PerformerId = source.Id, Target = target, Scope = scope, Group = group });
    }
}