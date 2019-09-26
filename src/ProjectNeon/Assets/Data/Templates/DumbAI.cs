using UnityEngine;

public sealed class DumbAI : TurnAI
{
    [SerializeField] private BattleState battleState;

    public DumbAI Init(BattleState state)
    {
        battleState = state;
        return this;
    }

    public override PlayedCard Play(Enemy activeEnemy)
    {
        var me = activeEnemy.AsMember();
        var card = activeEnemy.Deck.Cards.Random();
        var possibleTargets = battleState.GetPossibleEnemyTeamTargets(me, card.Actions[0].Group, card.Actions[0].Scope);
        var target = possibleTargets.Random();
        
        return new PlayedCard().Init(me, target, card);
    }

}
