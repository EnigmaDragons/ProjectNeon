using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/MissileCommander")]
public class MissileCommanderAI : TurnAI
{
    private List<IPlayedCard> _currentTurnPlayed = new List<IPlayedCard>();

    private int _currentTurnNumber;
    
    public override void InitForBattle()
    {
        _currentTurnPlayed = new List<IPlayedCard>();
    }
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        UpdateTurnTracking(battleState.TurnNumber);
        
        var card = new CardSelectionContext(me, battleState, strategy)
            .WithCommonSenseSelections()
            .IfTrueDontPlayType(_ => _currentTurnPlayed.Any(x => x.Card.Type.Is(CardTag.Exclusive)), CardTag.Exclusive)
            .WithSelectedUltimateIfAvailable()
            .WithFinalizedCardSelection(CardTag.BuffResource)
            .WithSelectedTargetsPlayedCard();
        _currentTurnPlayed.Add(card);
        return card;
    }

    private void UpdateTurnTracking(int currentTurnNumber)
    {
        if (currentTurnNumber == _currentTurnNumber)
            return;

        _currentTurnNumber = currentTurnNumber;
        _currentTurnPlayed = new List<IPlayedCard>();
    }
}
