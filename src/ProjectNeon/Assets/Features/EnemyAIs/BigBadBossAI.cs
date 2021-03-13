using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/BigBad")]
public class BigBadBossAI : StatefulTurnAI
{
    private static string Hide => "Hide Behind Big Hands";
    private static string BreakerPunch => "Breaker Punch";
    private static string DoubleFist => "Double Fist Mega Punch";
    private static string Retaliation => "Retaliation Stance";
    private static string ShieldBot => "Shield Me";
    private static string SpinSlam => "Spin Slam";
    
    private static readonly List<string[]> Routines = new List<string[]>
    {
        new [] { Hide, BreakerPunch, DoubleFist, SpinSlam },
        new [] { BreakerPunch, SpinSlam, ShieldBot, Hide },
        new [] { BreakerPunch, Retaliation, ShieldBot, Hide },
        new [] { Retaliation, BreakerPunch, DoubleFist, BreakerPunch }
    };

    private Queue<string> _currentRoutine;

    public override void InitForBattle()
    {
        _currentRoutine = new Queue<string>();
    }

    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {        
        SelectAttackRoutineIfNeeded();

        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedCardByNameIfPresent(_currentRoutine.Peek())
            .WithSelectedTargetsPlayedCard();
    }

    protected override void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy)
    {
        _currentRoutine.Dequeue();
    }
    
    private void SelectAttackRoutineIfNeeded()
    {
        if (_currentRoutine.Count > 0) return;
        
        var routine = Routines.Random();
        foreach (var card in routine)
            _currentRoutine.Enqueue(card);
    }
}
