using System;

public abstract class StatefulTurnAI : TurnAI
{
    public abstract override void InitForBattle();
    protected abstract IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy);
    protected abstract void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy);

    public sealed override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
        => WithTrackedState(SafeSelect(memberId, battleState, strategy), battleState, strategy);

    public sealed override IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy)
        => SafeSelect(memberId, battleState, strategy.AnticipationCopy);
    
    private IPlayedCard WithTrackedState(IPlayedCard card, BattleState state, AIStrategy strategy)
    {
        TrackState(card, state, strategy);
        return card;
    }
    
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
            return new PlayedCardV2(member, new Target[1]{new Single(member)}, new Card(battleState.GetNextCardId(), member, strategy.SpecialCards.AiGlitchedCard));
        }
    }
}
