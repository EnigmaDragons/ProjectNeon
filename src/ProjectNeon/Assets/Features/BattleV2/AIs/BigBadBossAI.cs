using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/BigBad")]
public class BigBadBossAI : TurnAI
{
    private static string Hide => "Hide Behind Big Hands";
    private static string BreakerPunch => "Breaker Punch";
    private static string DoubleFist => "Double Fist Mega Punch";
    private static string Retaliation => "Retaliation Stance";
    private static string ShieldBot => "Shield Me";
    private static string SpinSlam => "Spin Slam";
    
    private static readonly List<string[]> Routines = new List<string[]>
    {
        new [] { Hide, BreakerPunch, SpinSlam, DoubleFist },
        new [] { BreakerPunch, SpinSlam, Hide, ShieldBot },
        new [] { BreakerPunch, Retaliation, Hide, ShieldBot },
        new [] { Retaliation, BreakerPunch, BreakerPunch, DoubleFist }
    };
    
    private readonly Queue<string> _currentRoutine = new Queue<string>();

    public override void InitForBattle()
    {
        _currentRoutine.Clear();
    }
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        SelectAttackRoutineIfNeeded();

        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedCardByNameIfPresent(_currentRoutine.Dequeue())
            .WithSelectedTargetsPlayedCard();
    }

    private void SelectAttackRoutineIfNeeded()
    {
        if (_currentRoutine.Count > 0) return;
        
        var routine = Routines.Random();
        foreach (var card in routine)
            _currentRoutine.Enqueue(card);
    }
}
