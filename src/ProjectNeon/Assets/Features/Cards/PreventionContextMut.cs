using System.Collections.Generic;
using System.Linq;

public sealed class PreventionContextMut : PreventionContext
{
    private Member[] TargetMembers { get; }
    private Dictionary<PreventionType, HashSet<Member>> PreventingMembers { get; }

    public PreventionContextMut(Target target)
    {
        TargetMembers = target.Members;
        PreventingMembers = new Dictionary<PreventionType, HashSet<Member>>();
    }

    public Member[] GetPreventingMembersRelevantForDamageEffect()
    {
        PreventingMembers[PreventionType.Barrier] = new HashSet<Member>();
        TargetMembers.ForEach(m =>
        {
            if (m.State[TemporalStatType.Barrier] > 0)
                PreventingMembers[PreventionType.Barrier].Add(m);
        });
        return PreventingMembers[PreventionType.Barrier].ToArray();
    }
    
    public void UpdatePreventionCounters()
    {
        PreventingMembers.ForEach(kv => kv.Value.ForEach(m => m.Apply(ms => ms.Adjust(kv.Key.ToString(), -1f))));
    }
}
