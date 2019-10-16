using System;
using System.Linq;
using UnityEngine;

[Serializable]
public sealed class CardAction
{
    [SerializeField] private Scope targetScope;
    [SerializeField] private Group targetGroup;
    [SerializeField] private EffectData effect1;
    [SerializeField] private EffectData effect2;
    [SerializeField] private EffectData effect3;

    private EffectData[] Effects => Array.Empty<EffectData>()
        .ConcatIf(effect1, e => e.ShouldApply)
        .ConcatIf(effect2, e => e.ShouldApply)
        .ConcatIf(effect3, e => e.ShouldApply)
        .ToArray(); 
    
    public void Apply(Member source, Target target)
    {
        Effects.ForEach(
            effect => AllEffects.Apply(effect, source, target)
        );
    }

    public Scope Scope => targetScope;
    public Group Group => targetGroup;
    public bool HasEffects => Effects != null && Effects.Length > 0;
}
