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

    public IStats Origin => origin;
    public virtual int MaxHP => origin.MaxHP;
    public virtual int MaxShield => origin.MaxShield;
    public virtual int Attack => origin.Attack;
    public virtual int Magic => origin.Magic;
    public virtual float Armor => origin.Armor;
    public virtual float Resistance => origin.Resistance;
    public virtual IResourceType[] ResourceTypes => origin.ResourceTypes;

    public bool Active(int turn) {
        return turn < turnStarted + duration;
    }

    public void Init(IStats origin, int turnStarted, int duration)
    {
        this.origin = origin;
    }
}
