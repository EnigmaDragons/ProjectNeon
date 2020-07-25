using UnityEngine;

[SerializeField]
public class CardActionV2
{
    [SerializeField] private CardActionType type;
    [SerializeField] private EffectData battleEffect;

    public CardActionType Type => type;
    public void Apply(Member source, Target[] targets) => AllEffects.Apply(battleEffect, source, targets[battleEffect.TargetIndex]);
}