using System;
using UnityEngine;

public sealed class BattlePlayerTargetingState : ScriptableObject
{
    [SerializeField, ReadOnly] private int currentTarget;
    [SerializeField, ReadOnly] private int numTargets;
    [SerializeField] private GameEvent onTargetChanged;

    private Target[] _possibleTargets;
    
    public GameEvent OnTargetChanged => onTargetChanged;

    public BattlePlayerTargetingState WithPossibleTargets(Target[] targets)
    {
        _possibleTargets = targets;
        numTargets = targets.Length;
        currentTarget = 0;
        OnTargetChanged.Publish();
        return this;
    }

    public Target Current => _possibleTargets[currentTarget];
    
    public void MoveNext()
    {
        currentTarget = (currentTarget + 1) % numTargets;
        OnTargetChanged.Publish();
    }

    public void MovePrevious()
    {
        var next = (currentTarget - 1) % numTargets;
        if (next < 0)
            next = numTargets - 1;
        currentTarget = next;
        OnTargetChanged.Publish();
    }
}
