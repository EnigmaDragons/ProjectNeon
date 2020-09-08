using System.Collections.Generic;

public sealed class MemberStateSnapshot
{
    public int VersionNumber { get; }
    public int MemberId { get; }
    public IStats Stats { get; }
    public Dictionary<string, int> Counters { get; }
    // Maybe we could store Counter Maximums if needed. Could be relevant for MaxHP and MaxShield

    public MemberStateSnapshot(int versionNumber, int memberId, IStats stats, Dictionary<string, int> counters)
    {
        VersionNumber = versionNumber;
        MemberId = memberId;
        Stats = stats;
        Counters = counters;
    }

    public int Hp => Counters[TemporalStatType.HP.ToString()];
}
