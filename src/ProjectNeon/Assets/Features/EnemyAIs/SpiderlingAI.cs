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
        if (!_targetMap.ContainsKey(memberId) || !battleState.Members[_targetMap[memberId]].IsConscious())
        {
            var card = battleState.GetPlayableCards(memberId).First(x => x.Name == "Leaping Strike");
            var target = strategy.AttackTargetFor(card.ActionSequences[0]);
            return new PlayedCardV2(battleState.Members[memberId], target.AsArray(), card.CreateInstance(battleState.GetNextCardId(), battleState.Members[memberId]));
        }
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedCardByNameIfPresent("Envenom")
            .WithSelectedTargetsPlayedCard(t => t.Members[0].Id == _targetMap[memberId]);
    }

    protected override void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy)
    {
        _targetMap[card.MemberId()] = card.PrimaryTargetId();
    }
}