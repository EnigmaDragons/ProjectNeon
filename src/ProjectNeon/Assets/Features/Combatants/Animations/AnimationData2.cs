using System;
using System.Linq;

[Serializable]
public class AnimationData2
{
    public CharacterAnimationType Type;
    public StaticEffectCondition[] Conditions;
    
    public EffectCondition Condition
        => Conditions != null && Conditions.Length > 0 
            ? (EffectCondition) new AndEffectCondition(Conditions.Cast<EffectCondition>().ToArray()) 
            : new NoEffectCondition();
}