using System;
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
        if (type == PreventionType.Dodge)
        {
            if (!PreventingMembers.ContainsKey(PreventionType.Dodge))
                PreventingMembers[PreventionType.Dodge] = new HashSet<Member>();
            TargetMembers.Where(m => m.State[TemporalStatType.Dodge] > 0)
                .Intersect(members)
                .ForEach(m => PreventingMembers[PreventionType.Dodge].Add(m));
        }
    }

    public Member[] GetPreventingMembers(PreventionType type) 
        => PreventingMembers.TryGetValue(type, out var result) 
            ? result.ToArray() 
            : Array.Empty<Member>();

    public void UpdatePreventionCounters()
    {
        PreventingMembers.ForEach(kv => kv.Value.ForEach(m => m.Apply(ms => ms.Adjust(kv.Key.ToString(), -1f))));
    }
}
