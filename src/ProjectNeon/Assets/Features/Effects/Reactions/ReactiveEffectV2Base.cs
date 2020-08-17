using System;

public abstract class ReactiveEffectV2Base : ReactiveStateV2
{
    private int _remainingDurationTurns;
    private int _remainingUses;
    private readonly Func<EffectResolved, Maybe<ProposedReaction>> _createMaybeEffect;
    private bool HasMoreUses => _remainingUses != 0;
    private bool HasMoreTurns => _remainingDurationTurns != 0;

    public IStats Stats => new StatAddends();
    public bool IsDebuff { get; }
    public bool IsActive => HasMoreUses && HasMoreTurns;

    public ReactiveEffectV2Base(bool isDebuff, int maxDurationTurns, int maxUses, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
    {
        _remainingDurationTurns = maxDurationTurns;
        _remainingUses = maxUses;
        _createMaybeEffect = createMaybeEffect;
        IsDebuff = isDebuff;
    }
    
    public void OnTurnStart() {}

    public void OnTurnEnd()
    {
        if (_remainingDurationTurns > -1)
            _remainingDurationTurns--;
    }

    public abstract StatusTag Tag { get; }

    public Maybe<ProposedReaction> OnEffectResolved(EffectResolved e)
    {
        var maybeEffect = _createMaybeEffect(e);
        if (maybeEffect.IsPresent)
            _remainingUses--;
        return maybeEffect;
    }
}
