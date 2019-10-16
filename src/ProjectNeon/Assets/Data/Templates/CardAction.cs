using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class CardAction
{
    [SerializeField] private List<Effect> effects;
    [SerializeField] private Scope targetScope;
    [SerializeField] private Group targetGroup;
    
    public void Apply(Member source, Target target)
    {
        effects.ForEach(
            effect => effect.Apply(source, target)
        );
    }

    public Scope Scope => targetScope;

    public Group Group => targetGroup;
}
