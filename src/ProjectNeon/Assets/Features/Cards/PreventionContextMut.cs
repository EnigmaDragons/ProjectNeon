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
    
    private readonly Dictionary<PreventionType, TemporalStatType> _temporalStatForPrevention = new Dictionary<PreventionType, TemporalStatType>
    {
        { PreventionType.Dodge, TemporalStatType.Dodge },
        { PreventionType.Aegis, TemporalStatType.Aegis }
    };
    
    public void RecordPreventionTypeEffect(PreventionType type, params Member[] members)
    {
        if (!_temporalStatForPrevention.TryGetValue(type, out var temporalStatType))
            return;
        if (!PreventingMembers.ContainsKey(type))
            PreventingMembers[type] = new HashSet<Member>();
        TargetMembers.Where(m => m.State[temporalStatType] > 0)
            .Intersect(members)
            .ForEach(m => PreventingMembers[type].Add(m));
    }

    public Member[] GetPreventingMembers(PreventionType type) 
        => PreventingMembers.TryGetValue(type, out var result) 
            ? result.ToArray() 
            : Array.Empty<Member>();

    // This will be unecessary if we update the counter when the first usage is recorded
    public void UpdatePreventionCounters()
    {
        PreventingMembers.ForEach(kv => kv.Value.ForEach(m => m.Apply(ms => ms.Adjust(kv.Key.ToString(), -1f))));
    }
}
