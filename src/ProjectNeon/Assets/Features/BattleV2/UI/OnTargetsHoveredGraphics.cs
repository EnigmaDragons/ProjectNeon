using System.Linq;
using UnityEngine;

public class OnTargetsHoveredGraphics : OnMessage<TargetChanged, TargetSelectionFinished>
{
    [SerializeField] private GameObject targetingPrefab;
    [SerializeField] private BattleState battleState;

    private GameObject[] _targetingInstances = new GameObject[0];
    
    protected override void Execute(TargetChanged msg)
    {
        foreach (var instance in _targetingInstances)
            Destroy(instance);
        _targetingInstances = msg.Target.IsMissing
            ? new GameObject[0]
            : msg.Target.Value.Members.Select(x => Instantiate(targetingPrefab, battleState.GetTransform(x.Id))).ToArray();
    }

    protected override void Execute(TargetSelectionFinished msg)
    {
        foreach (var instance in _targetingInstances)
            Destroy(instance);
        _targetingInstances = new GameObject[0];
    }
}