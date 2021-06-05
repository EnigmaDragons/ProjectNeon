
public abstract class StatelessTurnAI : TurnAI
{
    public override void InitForBattle() {}

    public sealed override IPlayedCard InnerPlay(int memberId, BattleState battleState, AIStrategy strategy)
        => Select(memberId, battleState, strategy);

    public sealed override IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy)
        => Select(memberId, battleState, strategy.AnticipationCopy);

    protected abstract IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy);
}
