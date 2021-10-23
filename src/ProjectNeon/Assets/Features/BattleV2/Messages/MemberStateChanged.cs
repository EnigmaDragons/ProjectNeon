
public class MemberStateChanged
{
    public MemberState State { get; }
    public MemberStateSnapshot BeforeState { get; }

    public MemberStateChanged(MemberStateSnapshot before, MemberState after)
    {
        BeforeState = before;
        State = after;
    }
}

public static class MemberStateChangedExtensions
{
    public static int MemberId(this MemberStateChanged m) => m.State.MemberId;
    public static bool GainedHp(this MemberStateChanged m) => m.State.Hp() > m.BeforeState.Hp;
    public static bool LostHp(this MemberStateChanged m) => m.State.Hp() < m.BeforeState.Hp;
    public static bool GainedShield(this MemberStateChanged m) => m.State.Shield() > m.BeforeState.Shield;
    public static bool LostShield(this MemberStateChanged m) => m.State.Shield() < m.BeforeState.Shield;
    public static bool WasKnockedOut(this MemberStateChanged m) => m.State.IsUnconscious && m.BeforeState.Hp > 0;
    public static bool GainedStealth(this MemberStateChanged m) => m.State[TemporalStatType.Stealth] > 0 && m.BeforeState[TemporalStatType.Stealth] == 0;
    public static bool ExitedStealth(this MemberStateChanged m) => m.State[TemporalStatType.Stealth] == 0 && m.BeforeState[TemporalStatType.Stealth] > 0;
}
