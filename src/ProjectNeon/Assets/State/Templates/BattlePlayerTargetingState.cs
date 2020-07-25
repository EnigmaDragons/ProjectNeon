using System;
using UnityEngine;

public sealed class BattlePlayerTargetingState : ScriptableObject, IDirectionControllable
{
    [SerializeField] private GameEvent onTargetChanged;

    private IndexSelector<Target> _selector;
    
    [Obsolete] public GameEvent OnTargetChanged => onTargetChanged;

    public BattlePlayerTargetingState WithPossibleTargets(Target[] targets)
    {
        _selector = new IndexSelector<Target>(targets);
        OnTargetChanged.Publish();
        Message.Publish(new TargetChanged(Current));
        return this;
    }

    public Target Current => _selector.Current;

    public void Clear()
    {
        var emptyTargets = new Target[] { new Multiple(new Member[0]) };
        _selector = new IndexSelector<Target>(emptyTargets);
        OnTargetChanged.Publish();
        Message.Publish(new TargetChanged());
    }
    
    public void MoveNext()
    {
        _selector.MoveNext();
        OnTargetChanged.Publish();
        Message.Publish(new TargetChanged(Current));
    }

    public void MovePrevious()
    {
        _selector.MovePrevious();
        OnTargetChanged.Publish();
        Message.Publish(new TargetChanged(Current));
    }
}
