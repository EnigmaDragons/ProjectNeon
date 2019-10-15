using System.Linq;
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

    public override int MaxHP => maxHP;
    public override int MaxShield => maxShield;
    public override int Attack => attack;
    public override int Magic => magic;
    public override float Armor => armor;
    public override float Resistance => resistance;
    public override IResourceType[] ResourceTypes => resourceTypes.Cast<IResourceType>().ToArray();
    public override bool Active(int currentTurn) => true;
}
