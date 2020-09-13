using UnityEngine;

public abstract class TurnAI : ScriptableObject
{
    public virtual void InitForBattle() {}
    public abstract IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy);
}
