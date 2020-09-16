using System;

public sealed class PartyEffect : Effect
{
    private readonly Action<PartyAdventureState> _applyEffect;

    public PartyEffect(Action<PartyAdventureState> applyEffect) => _applyEffect = applyEffect;

    public void Apply(EffectContext ctx) => _applyEffect(ctx.AdventureState);
}
