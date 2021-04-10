using System;

public class UnpreventableContext : PreventionContext
{
    public Member[] GetPreventingMembersRelevantForDamageEffect() => Array.Empty<Member>();

    public void UpdatePreventionCounters() {}
}
