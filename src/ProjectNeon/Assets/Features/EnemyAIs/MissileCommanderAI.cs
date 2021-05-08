using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/MissileCommander")]
public class MissileCommanderAI : StatefulTurnAI
{
    private List<IPlayedCard> _currentTurnPlayed = new List<IPlayedCard>();

    private int _currentTurnNumber;
    
    public override void InitForBattle() => _currentTurnPlayed = new List<IPlayedCard>();

    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        UpdateTurnTracking(battleState.TurnNumber);
        
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithCommonSenseSelections()
            .IfTrueDontPlayType(_ => _currentTurnPlayed.Any(x => x.Card.Type.Is(CardTag.Exclusive)), CardTag.Exclusive)
            .WithSelectedUltimateIfAvailable()
            .WithFinalizedCardSelection(CardTag.BuffResource)
            .WithSelectedTargetsPlayedCard();
    }

    protected override void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy)
    {
        _currentTurnPlayed.Add(card);
    }
    
    private void UpdateTurnTracking(int currentTurnNumber)
    {
        if (currentTurnNumber == _currentTurnNumber)
            return;

        _currentTurnNumber = currentTurnNumber;
        _currentTurnPlayed = new List<IPlayedCard>();
    }
}
