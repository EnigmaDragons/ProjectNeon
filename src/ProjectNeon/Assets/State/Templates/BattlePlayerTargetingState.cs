using UnityEngine;

public sealed class BattlePlayerTargetingState : ScriptableObject
{
    [SerializeField] private Transform[] targetUiTransforms;
    [SerializeField] private int currentTarget;

    private Target[] possibleTargets;

    public BattlePlayerTargetingState WithPossibleTargets(Target[] targets)
    {
        possibleTargets = targets;
        currentTarget = 0;
        return this;
    }
}
