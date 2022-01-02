using System.Collections.Generic;
using System.Linq;

public sealed class MemberStateSnapshot
{
    public int VersionNumber { get; }
    public int MemberId { get; }
    public IStats Stats { get; }
    public Dictionary<string, int> Counters { get; }
    public IResourceType[] ResourceTypes { get; }
    public DictionaryWithDefault<CardTag, int> TagsPlayed { get; }
    public DictionaryWithDefault<StatusTag, int> StatusesOfType { get; }
    public StatType PrimaryStat { get; }
    // Maybe we could store Counter Maximums if needed. Could be relevant for MaxHP and MaxShield

    public MemberStateSnapshot(int versionNumber, int memberId, IStats stats, Dictionary<string, int> counters, IResourceType[] resourceTypes, 
        DictionaryWithDefault<CardTag, int> tagsPlayed, DictionaryWithDefault<StatusTag, int> statusesOfType, StatType primaryStat)
    {
        VersionNumber = versionNumber;
        MemberId = memberId;
        Stats = stats;
        Counters = counters;
        ResourceTypes = resourceTypes;
        TagsPlayed = tagsPlayed;
        StatusesOfType = statusesOfType;
        PrimaryStat = primaryStat;
    }
    
    public int this[IResourceType resourceType] => Counters.ValueOrDefault(resourceType.Name, 0);
    public float this[StatType statType] => Stats[statType];
    public int this[TemporalStatType statType] => Counters.VerboseGetValue(statType.ToString(), n => $"Counter '{n}'") + Stats[statType].CeilingInt();
    public int Hp => Counters[TemporalStatType.HP.ToString()];
    public int MaxHp => Stats[StatType.MaxHP].CeilingInt();
    public int MissingHp => MaxHp - Hp;
    public int Shield => this[TemporalStatType.Shield];
    public int MaxShield => Stats[StatType.MaxShield].CeilingInt();
    public int HpAndShield => Hp + Shield;
    public IResourceType PrimaryResource => ResourceTypes.Any() ? ResourceTypes[0] : new InMemoryResourceType();
    public int PrimaryResourceAmount => ResourceTypes.Any() ? Counters[PrimaryResource.Name] : 0;
}
