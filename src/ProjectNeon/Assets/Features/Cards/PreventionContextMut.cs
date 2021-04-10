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
    
    public void RecordPreventionTypeEffect(PreventionType type, params Member[] members)
    {
        if (type == PreventionType.Barrier)
        {
            if (!PreventingMembers.ContainsKey(PreventionType.Barrier))
                PreventingMembers[PreventionType.Barrier] = new HashSet<Member>();
            TargetMembers.Where(m => m.State[TemporalStatType.Barrier] > 0)
                .Intersect(members)
                .ForEach(m => PreventingMembers[PreventionType.Barrier].Add(m));
        }
    }

    public Member[] GetPreventingMembers(PreventionType type) => PreventingMembers[type].ToArray();

    public void UpdatePreventionCounters()
    {
        PreventingMembers.ForEach(kv => kv.Value.ForEach(m => m.Apply(ms => ms.Adjust(kv.Key.ToString(), -1f))));
    }
}
