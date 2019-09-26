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
        var possibleTargets = battleState.GetPossibleEnemyTeamTargets(me, card.TargetGroup, card.TargetScope);
        var target = possibleTargets.Random();
        
        return new PlayedCard().Init(me, target, card);
    }

}
