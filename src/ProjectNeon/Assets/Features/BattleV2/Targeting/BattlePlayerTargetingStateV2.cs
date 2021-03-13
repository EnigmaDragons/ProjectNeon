using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/BattlePlayerTargettingStateV2")]
public class BattlePlayerTargetingStateV2 : ScriptableObject
{
    [SerializeField] private BattleState battleState;

    private List<Dictionary<int, Target>> _memberToTargetMap = new List<Dictionary<int, Target>>();
    public Maybe<int> TargetMember { get; private set; }
    private List<Func<Target>> _getTargets;

    public bool HasValidTargets => !_memberToTargetMap.Any() || TargetMember.IsPresent;
    public Target[] Targets => _getTargets.Select(x => x()).ToArray();

    public void IndicateMember(int memberId)
    {
        if ((TargetMember.IsPresent && TargetMember.Value == memberId) || !_memberToTargetMap.Any())
            return;
        if (_memberToTargetMap.All(x => x.ContainsKey(memberId)))
        {
            TargetMember = memberId;
            Message.Publish(new TargetChanged(_memberToTargetMap[0][memberId]));
        }
        else
            StopIndicating();
    }

    public void StopIndicating()
    {
        if (!TargetMember.IsMissing)
        {
            TargetMember = Maybe<int>.Missing();
            Message.Publish(new TargetChanged());
        }
    }

    public void Init(List<Dictionary<int, Target>> memberToTargetMap, List<Func<Target>> getTargets)
    {
        TargetMember = Maybe<int>.Missing();
        _memberToTargetMap = memberToTargetMap;
        _getTargets = getTargets;
    }
}