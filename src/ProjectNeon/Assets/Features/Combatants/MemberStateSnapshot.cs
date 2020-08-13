using System.Collections.Generic;

public sealed class MemberStateSnapshot
{
    public int MemberId { get; }
    public IStats Stats { get; }
    public Dictionary<string, int> Counters { get; }
    // Maybe we could store Counter Maximums if needed. Could be relevant for MaxHP and MaxShield

    public MemberStateSnapshot(int memberId, IStats stats, Dictionary<string, int> counters)
    {
        MemberId = memberId;
        Stats = stats;
        Counters = counters;
    }
}
