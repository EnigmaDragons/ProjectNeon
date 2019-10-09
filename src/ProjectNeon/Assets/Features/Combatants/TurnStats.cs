using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Stats decorator for temporary stats modifications
 */
public abstract class TurnStats : IStats
{
    private IStats origin;
    private int turnStarted;
    private int duration;

    public int MaxHP => origin.MaxHP;
    public int MaxShield => origin.MaxShield;
    public int Attack => origin.Attack;
    public int Magic => origin.Magic;
    public float Armor => origin.Armor;
    public float Resistance => origin.Resistance;
    public IResourceType[] ResourceTypes => origin.ResourceTypes;

    public bool Active(int turn) {
        return turn < turnStarted + duration;
    }
}
