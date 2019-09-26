
using System.Collections.Generic;
using UnityEngine;

public class CardAction : ScriptableObject
{
    [SerializeField] private List<Effect> effects;
    [SerializeField] private Scope targetScope;

     // @todo #54:30min Create Scope for targeting. We need to define Scope logic so we can complete targeting logic.

    public void Apply(Target target)
    {
        effects.ForEach(
            effect => effect.Apply(target)
        );
    }
}
