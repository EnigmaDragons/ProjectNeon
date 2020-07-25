using System;
using UnityEngine;

[Serializable]
public class CardBattleActionV2
{
    [SerializeField] private CardBattleActionType type;
    [SerializeField] private EffectData battleEffect;

    public CardBattleActionType Type => type;
    public void Apply(Member source, Target[] targets) => AllEffects.Apply(battleEffect, source, targets[battleEffect.TargetIndex]);
}