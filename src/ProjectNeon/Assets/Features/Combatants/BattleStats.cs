using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Stats decorator that lasts entire battle
 */
public abstract class BattleStats : IStats
{
    private IStats origin;

    public int MaxHP => origin.MaxHP;
    public int MaxShield => origin.MaxShield;
    public int Attack => origin.Attack;
    public int Magic => origin.Magic;
    public float Armor => origin.Armor;
    public float Resistance => origin.Resistance;
    public IResourceType[] ResourceTypes => origin.ResourceTypes;
    public bool Active(int turn) => true;
}
