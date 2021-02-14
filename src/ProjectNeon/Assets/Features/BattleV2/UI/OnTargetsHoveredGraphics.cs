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
            : msg.Target.Value.Members.Select(x => InitTargetingPrefab(x.Id)).ToArray();
    }

    protected override void Execute(TargetSelectionFinished msg)
    {
        foreach (var instance in _targetingInstances)
            Destroy(instance);
        _targetingInstances = new GameObject[0];
    }

    private GameObject InitTargetingPrefab(int memberId)
    {
        var memberTransform = battleState.GetTransform(memberId);
        var centerPoint = memberTransform.GetComponentInChildren<CenterPoint>();
        var parent = centerPoint?.transform ?? memberTransform;
        var obj = Instantiate(targetingPrefab);
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        return obj;
    }
}