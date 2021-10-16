using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="GlobalEffects/CurrentGlobalEffects")]
public class CurrentGlobalEffects : ScriptableObject
{
    [SerializeField] private List<GlobalEffect> globalEffects = new List<GlobalEffect>();

    public GlobalEffect[] Value => globalEffects.ToArray();

    public void Clear() => PublishAfter(() => 
        globalEffects.Clear());

    public void Apply(GlobalEffect e)
    {
        e.Apply();
        Add(e);
    }
    
    public void Add(GlobalEffect e) => PublishAfter(() => 
        globalEffects.Add(e));

    public void Remove(GlobalEffect e) => PublishAfter(() => 
        globalEffects.Remove(e));

    private void PublishAfter(Action a)
    {
        a();
        Message.Publish(new GlobalEffectsUpdated(Value));
    }
}