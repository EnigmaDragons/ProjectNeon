using System;
using System.Collections.Generic;
using System.Linq;

public sealed class PreventionContextMut : PreventionContext
{
    private Member[] TargetMembers { get; set; }
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
    
    private readonly Dictionary<PreventionType, string> _preventionTypeWords = new Dictionary<PreventionType, string>
    {
        { PreventionType.Dodge, "Dodged!" },
        { PreventionType.Aegis, "Prevented!" }
    };

    public PreventionContext WithUpdatedTarget(Target target)
    {
        TargetMembers = target.Members;
        return this;
    }

    public void RecordPreventionTypeEffect(PreventionType type, params Member[] members)
    {
        if (!_temporalStatForPrevention.TryGetValue(type, out var temporalStatType))
            return;
        if (!PreventingMembers.ContainsKey(type))
            PreventingMembers[type] = new HashSet<Member>();
        TargetMembers.Where(m => m.State[temporalStatType] > 0)
            .Intersect(members)
            .ForEach(m =>
            {
                if (PreventingMembers[type].Contains(m)) 
                    return;
                
                m.Apply(s => s.Adjust(temporalStatType, -1));
                PreventingMembers[type].Add(m);
                Message.Publish(new DisplayCharacterWordRequested(m, _preventionTypeWords[type]));
            });
    }

    public Member[] GetPreventingMembers(PreventionType type) 
        => PreventingMembers.TryGetValue(type, out var result) 
            ? result.ToArray() 
            : Array.Empty<Member>();
}
