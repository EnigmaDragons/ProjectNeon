using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AssassinDrone")]
public class AssassinDroneAI : TurnAI
{
    private bool _hasAttackedLastTurn;
    private bool _hasStealthedLastTurn;
    
    public override void InitForBattle()
    {
        _hasAttackedLastTurn = Rng.Bool();
        _hasStealthedLastTurn = false;
    }

    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var card = new CardSelectionContext(memberId, battleState, strategy)
            .IfTrueDontPlayType(c => _hasAttackedLastTurn, CardTag.Attack)
            .IfTruePlayType(c => !_hasAttackedLastTurn, CardTag.Attack)
            .IfTrueDontPlayType(c => _hasStealthedLastTurn, CardTag.Stealth)
            .IfTrueDontPlayType(c => c.Enemies.All(x => x.BattleRole != BattleRole.Healer), CardTag.AntiHeal)
            .DontPlayShieldAttackIfOpponentsDontHaveManyShields(9)
            .WithFinalizedCardSelection();
        _hasAttackedLastTurn = card.SelectedCard.Value.Tags.Contains(CardTag.Attack);
        _hasStealthedLastTurn = card.SelectedCard.Value.Tags.Contains(CardTag.Stealth);
        return card.WithSelectedTargetsPlayedCard();
    }
}