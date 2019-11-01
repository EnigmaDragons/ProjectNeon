using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class Attack 
{
    public Member Attacker { get; }
    public Member Target { get; }
    public int Damage { get; }

    public Attack(Member attacker, Member target, int damage)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
    }
}
