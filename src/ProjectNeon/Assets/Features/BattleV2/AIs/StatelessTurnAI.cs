using System;

public abstract class StatelessTurnAI : TurnAI
{
    public override void InitForBattle() {}

    public sealed override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
        => SafeSelect(memberId, battleState, strategy);

    public sealed override IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy)
        => SafeSelect(memberId, battleState, strategy.AnticipationCopy);

    protected abstract IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy);

    private IPlayedCard SafeSelect(int memberId, BattleState battleState, AIStrategy strategy)
    {
        try
        {
            return Select(memberId, battleState, strategy);
        }
        catch (Exception e)
        {
            Log.Error(e);
            var member = battleState.Members[memberId];
            return new PlayedCardV2(member, new Target[1]{new Single(member)}, new Card(battleState.GetNextCardId(), member, strategy.SpecialCards.AiGlitchedCard), false);
        }
    }
}
