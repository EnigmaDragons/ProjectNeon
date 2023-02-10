using UnityEngine;

public abstract class StaticTargetedCardCondition : ScriptableObject, TargetedCardCondition
{
    [SerializeField] public bool inversed;
    
    public abstract bool ConditionMet(TargetedCardConditionContext ctx);
}