using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Spiderling")]
public class SpiderlingAI : TurnAI
{
    private Dictionary<int, int> _targetMap = new Dictionary<int, int>();
    
    public override void InitForBattle() => _targetMap = new Dictionary<int, int>();

    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        if (!_targetMap.ContainsKey(memberId) || !battleState.Members[_targetMap[memberId]].IsConscious())
        {
            var card = battleState.GetPlayableCards(memberId).First(x => x.Name == "Leaping Strike");
            var target = strategy.AttackTargetFor(card.ActionSequences[0]);
            _targetMap[memberId] = target.Members[0].Id;
            return new PlayedCardV2(battleState.Members[memberId], new [] { target }, card.CreateInstance(battleState.GetNextCardId(), battleState.Members[memberId]));
        }
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedCardByNameIfPresent("Envenom")
            .WithSelectedTargetsPlayedCard(t => t.Members[0].Id == _targetMap[memberId]);
    }
}