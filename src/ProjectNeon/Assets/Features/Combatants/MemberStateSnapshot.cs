using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public sealed class MemberStateSnapshot
{
    public int VersionNumber { get; private set; }
    public int MemberId { get; private set; }
    public IStats Stats { get; private set; }
    public IStats BaseStats { get; private set; }
    public Dictionary<string, int> Counters { get; private set; }
    public InMemoryResourceType[] ResourceTypes { get; private set; }
    public DictionaryWithDefault<CardTag, int> TagsPlayed { get; private set; }
    public DictionaryWithDefault<StatusTag, int> StatusesOfType { get; private set; }
    public StatType PrimaryStat { get; private set; }
    // Maybe we could store Counter Maximums if needed. Could be relevant for MaxHP and MaxShield

    public MemberStateSnapshot() : this(-1, -1, new StatAddends(), new StatAddends(), new Dictionary<string, int>(),
        Array.Empty<InMemoryResourceType>(), new DictionaryWithDefault<CardTag, int>(0),
        new DictionaryWithDefault<StatusTag, int>(0), StatType.Attack) {}

    private MemberStateSnapshot(int versionNumber, int memberId, IStats stats, IStats baseStats, Dictionary<string, int> counters, InMemoryResourceType[] resourceTypes, 
        DictionaryWithDefault<CardTag, int> tagsPlayed, DictionaryWithDefault<StatusTag, int> statusesOfType, StatType primaryStat) 
        => Initialized(versionNumber, memberId, stats, baseStats, counters, resourceTypes, tagsPlayed, statusesOfType, primaryStat);

    public MemberStateSnapshot Initialized(int versionNumber, int memberId, IStats stats, IStats baseStats,
        Dictionary<string, int> counters, InMemoryResourceType[] resourceTypes,
        DictionaryWithDefault<CardTag, int> tagsPlayed, DictionaryWithDefault<StatusTag, int> statusesOfType,
        StatType primaryStat)
    {
        VersionNumber = versionNumber;
        MemberId = memberId;
        Stats = stats;
        BaseStats = baseStats;
        Counters = counters;
        ResourceTypes = resourceTypes;
        TagsPlayed = tagsPlayed;
        StatusesOfType = statusesOfType;
        PrimaryStat = primaryStat;
        return this;
    }
    
    public int this[IResourceType resourceType] => Counters.ValueOrDefault(resourceType.Name, 0);
    public float this[StatType statType] => Stats[statType];
    public int this[TemporalStatType statType] => Counters.VerboseGetValue(statType.GetString(), n => $"Counter '{n}'") + Stats[statType].CeilingInt();
    public int Hp => Counters[TemporalStatType.HP.GetString()];
    public int MaxHp => Stats[StatType.MaxHP].CeilingInt();
    public int MissingHp => MaxHp - Hp;
    public int Shield => this[TemporalStatType.Shield];
    public int OverkillDamageAmount => this[TemporalStatType.OverkillDamageAmount];
    public int MaxShield => Stats[StatType.MaxShield].CeilingInt();
    public int HpAndShield => Hp + Shield;
    public int HpAndShieldWithOverkill => Hp + Shield - OverkillDamageAmount;
    public IResourceType PrimaryResource => ResourceTypes.Any() ? ResourceTypes[0] : new InMemoryResourceType();
    public int PrimaryResourceAmount => ResourceTypes.Any() && Counters.ContainsKey(PrimaryResource.Name) ? Counters[PrimaryResource.Name] : 0;
}

public static class MemberStateSnapshotExtensions
{
    private static int _poolIndex = 0;
    private static readonly MemberStateSnapshot[] Pool = Enumerable.Range(0, ObjectPoolSizes.MemberStateSnapshotPoolSize).Select(_ => new MemberStateSnapshot()).ToArray();
    
    private static MemberStateSnapshot GetNext()
    {
        _poolIndex = (_poolIndex + 1) % Pool.Length;
        return Pool[_poolIndex];
    }

    public static bool Init = true;
    
    public static MemberStateSnapshot Create(int versionNumber, int memberId, IStats stats, IStats baseStats,
        Dictionary<string, int> counters, InMemoryResourceType[] resourceTypes, DictionaryWithDefault<CardTag, int> tagsPlayed,
        DictionaryWithDefault<StatusTag, int> statusesOfType, StatType primaryStat) 
            => GetNext().Initialized(versionNumber, memberId, stats, baseStats, counters, resourceTypes, tagsPlayed, statusesOfType, primaryStat);
}
