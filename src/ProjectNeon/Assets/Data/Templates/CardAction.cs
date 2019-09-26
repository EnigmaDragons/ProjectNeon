
using System.Collections.Generic;
using UnityEngine;

public class CardAction : ScriptableObject
{
    [SerializeField] private List<Effect> effects;
    [SerializeField] private Scope targetScope;
    [SerializeField] private Group targetGroup;
    
    public void Apply(Target target)
    {
        effects.ForEach(
            effect => effect.Apply(target)
        );
    }

    public Scope Scope
    {
        get { return targetScope; }
    }

    public Group Group
    {
        get { return targetGroup; }
    }

}
