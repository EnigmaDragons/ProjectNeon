using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Spiderling")]
public class SpiderlingAI : StatefulTurnAI
{
    private Dictionary<int, int> _targetMap = new Dictionary<int, int>();

    public override void InitForBattle()
    {
        _targetMap = new Dictionary<int, int>();
    }

    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        if (!_targetMap.ContainsKey(memberId) 
            || !battleState.Members[_targetMap[memberId]].IsConscious() 
            || battleState.Members[_targetMap[memberId]].IsStealthed()
            || (!battleState.Members[_targetMap[memberId]].HasTaunt() && battleState.MembersWithoutIds.Any(x => x.TeamType == TeamType.Party && x.Id != _targetMap[memberId] && x.HasTaunt() && x.IsConscious())))
        {
            var card = battleState.GetPlayableCards(memberId, battleState.Party, strategy.SpecialCards).FirstOrDefault(x => x.Name == "Leaping Strike");
            if (card == null)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedTargetsPlayedCard();
            
            var possibleTargets = battleState.GetPossibleConsciousTargets(me, card.ActionSequences[0].Group, card.ActionSequences[0].Scope);
            var target = strategy.AttackTargetFor(possibleTargets, card.ActionSequences[0]);
            return new PlayedCardV2(battleState.Members[memberId], target.AsArray(), card.CreateInstance(battleState.GetNextCardId(), battleState.Members[memberId]));
        }
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedCardByNameIfPresent("Envenom")
            .WithSelectedTargetsPlayedCard(t => t.Members[0].Id == _targetMap[memberId]);
    }

    protected override void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy)
    {
        if (card.Targets.Any() && card.Targets[0].Members.Any())
            _targetMap[card.MemberId()] = card.PrimaryTargetId();
        else
            Log.Warn($"{card.Card.Name} was played with targets {card.Targets}");
    }
}