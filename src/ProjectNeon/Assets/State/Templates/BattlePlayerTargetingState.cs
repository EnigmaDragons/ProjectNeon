using UnityEngine;

public sealed class BattlePlayerTargetingState : ScriptableObject
{
    [SerializeField] private GameEvent onTargetChanged;

    private IndexSelector<Target> _selector;
    
    public GameEvent OnTargetChanged => onTargetChanged;

    public BattlePlayerTargetingState WithPossibleTargets(Target[] targets)
    {
        _selector = new IndexSelector<Target>(targets);
        OnTargetChanged.Publish();
        return this;
    }

    public Target Current => _selector.Current;

    public void Clear()
    {
        var emptyTargets = new Target[] { new Multiple(new Member[0]) };
        _selector = new IndexSelector<Target>(emptyTargets);
        OnTargetChanged.Publish();
    }
    
    public void MoveNext()
    {
        _selector.MoveNext();
        OnTargetChanged.Publish();
    }

    public void MovePrevious()
    {
        _selector.MovePrevious();
        OnTargetChanged.Publish();
    }
}
