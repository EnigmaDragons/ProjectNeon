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
            : msg.Target.Value.Members.Select(x => InitTargetingPrefab(x.Id))
                .Where(x => x.IsPresent)
                .Select(x => x.Value).ToArray();
    }

    protected override void Execute(TargetSelectionFinished msg)
    {
        foreach (var instance in _targetingInstances)
            Destroy(instance);
        _targetingInstances = new GameObject[0];
    }

    private Maybe<GameObject> InitTargetingPrefab(int memberId)
    {
        var maybeMemberTransform = battleState.GetMaybeTransform(memberId);
        if (maybeMemberTransform.IsMissing)
            return Maybe<GameObject>.Missing();

        var memberTransform = maybeMemberTransform.Value;
        var centerPoint = memberTransform.GetComponentInChildren<CenterPoint>();
        var parent = centerPoint?.transform ?? memberTransform;
        var obj = Instantiate(targetingPrefab);
        // These next two lines are happening separate from instantiate to prevent weird scaling
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        return obj;
    }
}