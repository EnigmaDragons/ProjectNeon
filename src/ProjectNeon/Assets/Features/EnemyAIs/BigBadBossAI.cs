using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/BigBad")]
public class BigBadBossAI : RoutineTurnAI
{
    private static string Hide => "Hide Behind Big Hands";
    private static string BreakerPunch => "Breaker Punch";
    private static string DoubleFist => "Double Fist Mega Punch";
    private static string Retaliation => "Retaliation Stance";
    private static string ShieldBot => "Shield Me";
    private static string SpinSlam => "Spin Slam";
    
    private static Queue<string>[] Routines => new Queue<string>[]
    {
        new Queue<string>(new [] { Hide, BreakerPunch, DoubleFist, SpinSlam }),
        new Queue<string>(new [] { BreakerPunch, SpinSlam, ShieldBot, Hide }),
        new Queue<string>(new [] { BreakerPunch, Retaliation, ShieldBot, Hide }),
        new Queue<string>(new [] { Retaliation, BreakerPunch, DoubleFist, BreakerPunch})
    };

    protected override Queue<string> ChooseRoutine(CardSelectionContext ctx)
        => Routines.Random();
}
