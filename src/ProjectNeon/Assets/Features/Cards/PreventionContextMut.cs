using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        Log.Info($"Prevention {type}: {string.Join(", ", members.Select(m => m.Name))}");
        if (type == PreventionType.Dodge)
        {
            if (!PreventingMembers.ContainsKey(PreventionType.Dodge))
                PreventingMembers[PreventionType.Dodge] = new HashSet<Member>();
            TargetMembers.Where(m => m.State[TemporalStatType.Dodge] > 0)
                .Intersect(members)
                .ForEach(m => PreventingMembers[PreventionType.Dodge].Add(m));
            PreventingMembers[PreventionType.Dodge].ForEach(m => Log.Info($"Preventing - Dodge {m.Name}"));
        }
    }

    public Member[] GetPreventingMembers(PreventionType type) => PreventingMembers[type].ToArray();

    public void UpdatePreventionCounters()
    {
        PreventingMembers.ForEach(kv => kv.Value.ForEach(m => m.Apply(ms => ms.Adjust(kv.Key.ToString(), -1f))));
    }
}
