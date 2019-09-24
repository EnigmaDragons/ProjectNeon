using System;
using UnityEngine;

public sealed class BaseStats : Stats
{
    [SerializeField] private int maxHP;
    [SerializeField] private int maxShield;
    [SerializeField] private int attack;
    [SerializeField] private int magic;
    [SerializeField] private float armor;
    [SerializeField] private float resistance;
    [SerializeField] private ResourceType[] resourceTypes;

    public override int MaxHP => throw new NotImplementedException();
    public override int MaxShield => throw new NotImplementedException();
    public override int Attack => throw new NotImplementedException();
    public override int Magic => throw new NotImplementedException();
    public override float Armor => throw new NotImplementedException();
    public override float Resistance => throw new NotImplementedException();
    public override IResourceType[] ResourceTypes => throw new NotImplementedException();
}
