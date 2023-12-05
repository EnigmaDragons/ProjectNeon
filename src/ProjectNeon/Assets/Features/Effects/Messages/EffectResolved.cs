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
    public PreventionContext Preventions { get; } public ReactionTimingWindow Timing { get; }
    public CardPlayZones CardZones { get; }
    public Maybe<Card> OriginatingCard { get; }
   public Card[] CycledCards { get; }
    public Card[] DrawnCards { get; }

    public EffectResolved(bool wasApplied, bool isFirstBattleEffectOfChosenTarget, EffectData e, Member src, Target target, BattleStateSnapshot before, BattleStateSnapshot after, 
        bool isReaction, Maybe<Card> originatingCard, Card[] cycledCards, Card[] drawnCards, PreventionContext preventions, ReactionTimingWindow timing, CardPlayZones cardZones)
    {
        WasApplied = wasApplied;
        IsFirstBattleEffectOfChosenTarget = isFirstBattleEffectOfChosenTarget;
        EffectData = e;
        Source = src;
        Target = target;
        BattleBefore = before;
        BattleAfter = after;
        IsReaction = isReaction;
        OriginatingCard = originatingCard;
        CycledCards = cycledCards;
        DrawnCards = drawnCards;
        Preventions = preventions;
        Timing = timing;
        CardZones = cardZones;
    }
}

