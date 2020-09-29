using UnityEngine;
using UnityEditor;

public sealed class ApplyOnChance : ConditionalEffect
{
    private Effect _effect;
    private float _chance;

    public ApplyOnChance(Effect effect, float chance) : base (effect) {
        _chance = chance;
    }

    public override bool Condition()
    {
        return Rng.Dbl() <= _chance;
    }
}
