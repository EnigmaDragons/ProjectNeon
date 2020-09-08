using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Hypervisor")]
public class HypervisorAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        var allies = me.TeamType == TeamType.Enemies 
            ? battleState.Enemies.Where(m => m.IsConscious()).ToArray() 
            : battleState.Heroes.Where(m => m.IsConscious()).ToArray();
        
        var ctx = new CardSelectionContext(me, battleState, strategy)
            .WithOptions(playableCards)
            .WithSelectedDesignatedAttackerCardIfApplicable();
        if (ctx.SelectedCard.IsPresent)
        {
            var selectedCard = ctx.SelectedCard.Value;
            return new PlayedCardV2(me, selectedCard.ActionSequences.Select(strategy.AttackTargetFor).ToArray(),
                selectedCard.CreateInstance(battleState.GetNextCardId(), me));
        }

        if (playableCards.MostExpensive().Cost.Amount > 1 && allies.Any(x => x.CurrentHp() + x.CurrentShield() > x.MaxHp() * 0.5f))
        {
            var viableAllies = allies.Where(x => x.CurrentHp() + x.CurrentShield() > x.MaxHp() * 0.5f).ToArray();
            var ultimate = playableCards.MostExpensive().CreateInstance(battleState.GetNextCardId(), me);
            if (viableAllies.Any(x => x.BattleRole == BattleRole.Striker))
                return new PlayedCardV2(me, new Target[] { new Single(viableAllies.Where(x => x.BattleRole == BattleRole.Striker).Random()) }, ultimate);
            if (viableAllies.Any(x => x.BattleRole == BattleRole.Tank))
                return new PlayedCardV2(me, new Target[] { new Single(viableAllies.Where(x => x.BattleRole == BattleRole.Tank).Random()) }, ultimate);
            return new PlayedCardV2(me, new Target[] { new Single(viableAllies.Random()) }, ultimate);
        }

        if (allies.Length == 1)
        {
            var attackCard = playableCards.Where(c => c.Is(CardTag.Attack)).Random()
                .CreateInstance(battleState.GetNextCardId(), me);
            return new PlayedCardV2(me, attackCard.ActionSequences.Select(strategy.AttackTargetFor).ToArray(), attackCard);
        }
        else
            playableCards = playableCards.Where(x => !x.Is(CardTag.Attack)).ToArray();
        if (allies.Length == 2)
            playableCards = playableCards.Where(cardType => cardType.ActionSequences.Any(sequence => sequence.Group != Group.Ally || sequence.Scope != Scope.All)).ToArray();
        var card = playableCards.Random();
        return new PlayedCardV2(me, card.ActionSequences.Select(action =>
        {
            if (action.Group == Group.Ally && action.Scope == Scope.One)
            {
                var weightedList = new List<Member>();
                foreach (var ally in allies.Where(x => x != me))
                    Enumerable.Range(0, Mathf.RoundToInt(Mathf.Pow(((ally.CurrentHp() + ally.CurrentShield()) / (float)ally.MaxHp()) * 10, 1.6f) * (ally.BattleRole == BattleRole.Striker ? 3 : 1))).ForEach(x => weightedList.Add(ally));
                return new Single(weightedList.Random());
            }
            return strategy.GetRandomApplicableTarget(me, battleState.Members.Values.Where(x => x.IsConscious()).ToArray(), action);
        }).ToArray(), card.CreateInstance(battleState.GetNextCardId(), me));
    }
}
