using System;

public class UnpreventableContext : PreventionContext
{
    public void RecordPreventionTypeEffect(PreventionType type, params Member[] members) {}
    public Member[] GetPreventingMembers(PreventionType type) => Array.Empty<Member>();
}
