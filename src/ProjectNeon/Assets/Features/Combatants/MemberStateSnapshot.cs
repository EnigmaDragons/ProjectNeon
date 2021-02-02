using System.Collections.Generic;

public sealed class MemberStateSnapshot
{
    public int VersionNumber { get; }
    public int MemberId { get; }
    public IStats Stats { get; }
    public Dictionary<string, int> Counters { get; }
    public IResourceType[] ResourceTypes { get; }
    // Maybe we could store Counter Maximums if needed. Could be relevant for MaxHP and MaxShield

    public MemberStateSnapshot(int versionNumber, int memberId, IStats stats, Dictionary<string, int> counters, IResourceType[] resourceTypes)
    {
        VersionNumber = versionNumber;
        MemberId = memberId;
        Stats = stats;
        Counters = counters;
        ResourceTypes = resourceTypes;
    }
    
    public int this[IResourceType resourceType] => Counters.ValueOrDefault(resourceType.Name, 0);
    public float this[StatType statType] => Stats[statType];
    public int this[TemporalStatType statType] => Counters.VerboseGetValue(statType.ToString(), n => $"Counter '{n}'") + Stats[statType].CeilingInt();
    public int Hp => Counters[TemporalStatType.HP.ToString()];
    public int MaxHp => Stats[StatType.MaxHP].CeilingInt();
    public int Shield => Counters[TemporalStatType.Shield.ToString()];
}
