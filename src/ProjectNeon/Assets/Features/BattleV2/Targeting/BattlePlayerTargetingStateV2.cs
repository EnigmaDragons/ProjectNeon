using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/BattlePlayerTargettingStateV2")]
public class BattlePlayerTargetingStateV2 : ScriptableObject
{
    [SerializeField] private BattleState battleState;
    
    public List<Dictionary<int, Target>> MemberToTargetMap { get; set; }
    public Maybe<int> TargetMember { get; set; }
    public List<Func<Target>> GetTargets { get; set; }

    public bool HasValidTargets => !MemberToTargetMap.Any() || TargetMember.IsPresent;
    public Target[] Targets => GetTargets.Select(x => x()).ToArray();

    public void IndicateMember(int memberId)
    {
        if ((TargetMember.IsPresent && TargetMember.Value == memberId) || !MemberToTargetMap.Any())
            return;
        if (MemberToTargetMap.All(x => x.ContainsKey(memberId)))
        {
            TargetMember = memberId;
            Message.Publish(new TargetChanged(MemberToTargetMap[0][memberId]));
        }
        else if (!TargetMember.IsMissing)
        {
            TargetMember = Maybe<int>.Missing();
            Message.Publish(new TargetChanged());
        }
    }

    public void StopIndicating() => TargetMember = Maybe<int>.Missing();
}