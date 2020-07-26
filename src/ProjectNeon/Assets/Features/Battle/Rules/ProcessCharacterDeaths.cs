using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ProcessCharacterDeaths : GameEventActionScript
{
    [SerializeField] private BattleState state;

    [ReadOnly, SerializeField] private List<Member> _unconscious = new List<Member>(); 
    
    protected override void Execute() =>
        state.Members.Values.ToList()
            .Except(_unconscious)
            .Where(m => !m.State.IsConscious)
            .ForEach(m =>
            {    
                _unconscious.Add(m);
                Message.Publish(new MemberUnconscious(m));
            });
}
