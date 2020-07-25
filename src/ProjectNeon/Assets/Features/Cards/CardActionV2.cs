using UnityEngine;

[SerializeField]
public class CardActionV2
{
    [SerializeField] private CardActionType type;
    [SerializeField] private EffectData battleEffect;

    public void Apply(Member source, Target target) => AllEffects.Apply(battleEffect, source, target);
}