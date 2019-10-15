using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Stats decorator that lasts entire battle
 */
public abstract class BattleStats : IStats
{
    private IStats origin;

    public IStats Origin => origin;
    public virtual int MaxHP => origin.MaxHP;
    public virtual int MaxShield => origin.MaxShield;
    public virtual int Attack => origin.Attack;
    public virtual int Magic => origin.Magic;
    public virtual float Armor => origin.Armor;
    public virtual float Resistance => origin.Resistance;
    public virtual IResourceType[] ResourceTypes => origin.ResourceTypes;
    public bool Active(int turn) => true;

    public void Init(IStats origin)
    {
        this.origin = origin;
    }
}
