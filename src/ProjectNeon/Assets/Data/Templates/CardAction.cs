
using System.Collections.Generic;
using UnityEngine;

public class CardAction<T> : MonoBehaviour where T : Target
{
    [SerializeField] private List<Effect> effects;

    public void Apply(Target target)
    {
        effects.ForEach(
            effect => effect.Apply(target)
        );
    }
}
