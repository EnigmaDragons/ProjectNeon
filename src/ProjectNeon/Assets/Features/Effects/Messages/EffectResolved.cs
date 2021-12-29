public sealed class EffectResolved
{
    public bool WasApplied { get; }
    public bool IsFirstBattleEffect { get; }
    public EffectData EffectData { get; }
    public Member Source { get; }
    public Target Target { get; }
    public BattleStateSnapshot BattleBefore { get; }
    public BattleStateSnapshot BattleAfter { get; }
    public bool IsReaction { get; }
    public Maybe<Card> Card { get; }
    public PreventionContext Preventions { get; }

    public EffectResolved(bool wasApplied, bool isFirstBattleEffect, EffectData e, Member src, Target target, BattleStateSnapshot before, BattleStateSnapshot after, bool isReaction, Maybe<Card> card, PreventionContext preventions)
    {
        WasApplied = wasApplied;
        IsFirstBattleEffect = isFirstBattleEffect;
        EffectData = e;
        Source = src;
        Target = target;
        BattleBefore = before;
        BattleAfter = after;
        IsReaction = isReaction;
        Card = card;
        Preventions = preventions;
    }
}
