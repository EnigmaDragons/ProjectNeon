using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AssassinDrone")]
public class AssassinDroneAI : TurnAI
{
    private DictionaryWithDefault<int, bool> _hasAttackedLastTurn;
    private DictionaryWithDefault<int, bool> _hasStealthedLastTurn;
    
    public override void InitForBattle()
    {
        _hasAttackedLastTurn = new DictionaryWithDefault<int, bool>(Rng.Bool());
        _hasStealthedLastTurn = new DictionaryWithDefault<int, bool>(false);
    }

    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy) 
        => WithTrackedState(Anticipate(memberId, battleState, strategy));

    public override IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var card = new CardSelectionContext(memberId, battleState, strategy)
            .IfTrueDontPlayType(c => _hasAttackedLastTurn[memberId], CardTag.Attack)
            .IfTruePlayType(c => !_hasAttackedLastTurn[memberId], CardTag.Attack)
            .IfTrueDontPlayType(c => _hasStealthedLastTurn[memberId], CardTag.Stealth)
            .IfTrueDontPlayType(c => c.Enemies.All(x => x.BattleRole != BattleRole.Healer), CardTag.AntiHeal)
            .DontPlayShieldAttackIfOpponentsDontHaveManyShields(9)
            .WithFinalizedCardSelection();
        return card.WithSelectedTargetsPlayedCard();
    }
    
    private IPlayedCard WithTrackedState(IPlayedCard played)
    {
        _hasAttackedLastTurn[played.Member.Id] = played.Card.Type.Tags.Contains(CardTag.Attack);
        _hasStealthedLastTurn[played.Member.Id] = played.Card.Type.Tags.Contains(CardTag.Stealth);
        return played;
    }
}