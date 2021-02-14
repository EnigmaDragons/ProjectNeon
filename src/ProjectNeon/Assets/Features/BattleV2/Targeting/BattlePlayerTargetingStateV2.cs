using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlePlayerTargetingStateV2 : ScriptableObject
{
    [SerializeField] private BattleState battleState;
    
    public List<Dictionary<int, Target>> MemberToTargetMap { get; set; }
    public Maybe<int> TargetMember { get; set; }
    public List<Func<Target>> GetTargets { get; set; }

    public bool HasValidTargets => !MemberToTargetMap.Any() || TargetMember.IsPresent;
    public Target[] Targets => GetTargets.Select(x => x()).ToArray();

    public void InitCardForSelection(Card card)
    {
        GetTargets = new List<Func<Target>>();
        MemberToTargetMap = new List<Dictionary<int, Target>>();
        TargetMember = Maybe<int>.Missing();
        var targetMaps = 0;
        foreach (var sequence in card.ActionSequences)
        {
            if (sequence.Group == Group.All || sequence.Scope == Scope.All || sequence.Scope == Scope.AllExceptSelf)
            {
                var target = battleState.GetPossibleConsciousTargets(card.Owner, sequence.Group, sequence.Scope).First();
                GetTargets.Add(() => target);
            }
            else
            {
                var tmp = targetMaps;
                GetTargets.Add(() => MemberToTargetMap[tmp][TargetMember.Value]);
                var scopeOne = battleState.GetPossibleConsciousTargets(card.Owner, sequence.Group, Scope.One);
                MemberToTargetMap.Add(battleState.GetPossibleConsciousTargets(card.Owner, sequence.Group, sequence.Scope)
                    .ToDictionary(x =>
                    {
                        if (sequence.Scope == Scope.One || sequence.Scope == Scope.OneExceptSelf)
                            return x.Members[0].Id;
                        if (sequence.Scope == Scope.AllExcept)
                            return scopeOne.First(targeted => x.Members.All(member => member.Id != targeted.Members[0].Id)).Members[0].Id;
                        throw new Exception($"Scope {sequence.Scope} not supported for card {sequence.Group}");
                    }, x => x));
                targetMaps++;
            }
        }
    }

    public void IndicateMember(int memberId) => TargetMember = MemberToTargetMap.All(x => x.ContainsKey(memberId)) ? memberId : Maybe<int>.Missing();

    public void StopIndicating() => TargetMember = Maybe<int>.Missing();
}