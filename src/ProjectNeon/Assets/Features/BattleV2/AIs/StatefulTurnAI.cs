
public abstract class StatefulTurnAI : TurnAI
{
    public abstract override void InitForBattle();
    protected abstract IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy);
    protected abstract void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy);

    public sealed override IPlayedCard InnerPlay(int memberId, BattleState battleState, AIStrategy strategy)
        => WithTrackedState(Select(memberId, battleState, strategy), battleState, strategy);

    public sealed override IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy)
        => Select(memberId, battleState, strategy.AnticipationCopy);
    
    private IPlayedCard WithTrackedState(IPlayedCard card, BattleState state, AIStrategy strategy)
    {
        TrackState(card, state, strategy);
        return card;
    }
}
