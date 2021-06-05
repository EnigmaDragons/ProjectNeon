using UnityEngine;

public abstract class TurnAI : ScriptableObject
{
    public Maybe<IPlayedCard> LockedInDecision { get; set; }
    
    public virtual void InitForBattle() {}

    public IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        if (LockedInDecision.IsPresent)
        {
            var decision = LockedInDecision.Value;
            LockedInDecision = Maybe<IPlayedCard>.Missing();
            return decision;
        }
        return InnerPlay(memberId, battleState, strategy);
    }
    public abstract IPlayedCard InnerPlay(int memberId, BattleState battleState, AIStrategy strategy);
    public abstract IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy);
}
