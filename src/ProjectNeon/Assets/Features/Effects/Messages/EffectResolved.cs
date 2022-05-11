public sealed class EffectResolved
{
    public bool WasApplied { get; }
    public bool IsFirstBattleEffectOfChosenTarget { get; }
    public EffectData EffectData { get; }
    public Member Source { get; }
    public Target Target { get; }
    public BattleStateSnapshot BattleBefore { get; }
    public BattleStateSnapshot BattleAfter { get; }
    public bool IsReaction { get; }
    public Maybe<Card> Card { get; }
    public PreventionContext Preventions { get; }
    public ReactionTimingWindow Timing { get; }

    public EffectResolved(bool wasApplied, bool isFirstBattleEffectOfChosenTarget, EffectData e, Member src, Target target, BattleStateSnapshot before, BattleStateSnapshot after, 
        bool isReaction, Maybe<Card> card, PreventionContext preventions, ReactionTimingWindow timing)
    {
        WasApplied = wasApplied;
        IsFirstBattleEffectOfChosenTarget = isFirstBattleEffectOfChosenTarget;
        EffectData = e;
        Source = src;
        Target = target;
        BattleBefore = before;
        BattleAfter = after;
        IsReaction = isReaction;
        Card = card;
        Preventions = preventions;
        Timing = timing;
    }
}
