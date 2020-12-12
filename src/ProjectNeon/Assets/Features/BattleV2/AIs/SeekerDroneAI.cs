using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/SeekerDrone")]
public class SeekerDroneAI : TurnAI
{
    private Dictionary<int, int> _targetMap;
    
    public override void InitForBattle() => _targetMap = new Dictionary<int, int>();

    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        if (battleState.Members[memberId].PrimaryResourceAmount() >= 3)
            return new CardSelectionContext(memberId, battleState, strategy)
                .WithSelectedUltimateIfAvailable()
                .WithSelectedTargetsPlayedCard();
        if (!_targetMap.ContainsKey(memberId) || !battleState.Members[_targetMap[memberId]].IsConscious())
        {
            var card = battleState.GetPlayableCards(memberId).First(x => x.Name == "Target Locked");
            var target = strategy.AttackTargetFor(card.ActionSequences[0]);
            _targetMap[memberId] = target.Members[0].Id;
            return new PlayedCardV2(battleState.Members[memberId], new [] { target }, card.CreateInstance(battleState.GetNextCardId(), battleState.Members[memberId]));
        }
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedCardByNameIfPresent("Detonate")
            .WithSelectedTargetsPlayedCard(t => t.Members[0].Id == _targetMap[memberId]);
    }
}