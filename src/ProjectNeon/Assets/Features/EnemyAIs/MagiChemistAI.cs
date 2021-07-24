using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/MagiChemist")]
public class MagiChemistAI : TurnAI
{
    private DictionaryWithDefault<int, bool> _hasAttackedLastTurn = new DictionaryWithDefault<int, bool>(false);
    
    public override void InitForBattle() => _hasAttackedLastTurn = new DictionaryWithDefault<int, bool>(false);

    private IPlayedCard WithTrackedState(IPlayedCard played)
    {
        _hasAttackedLastTurn[played.Member.Id] = played.Card.Type.Is(CardTag.Attack);
        return played;
    }

    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
        => WithTrackedState(Select(memberId, battleState, strategy));

    public override IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy)
        => Select(memberId, battleState, strategy);

    private IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var ultimateTarget = battleState.GetPossibleConsciousTargets(battleState.Members[memberId], Group.Opponent, Scope.One)
            .Shuffled()
            .OrderByDescending(x => x.Members[0].CurrentHp())
            .FirstOrDefault();
        var card = new CardSelectionContext(memberId, battleState, strategy)
            .IfTrueDontPlayType(ctx => ctx.Member.CurrentHp() == ctx.Member.MaxHp() || ultimateTarget == null || ultimateTarget.Members[0].CurrentHp() + 15 < ctx.Member.CurrentHp(), CardTag.Ultimate)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .IfTrueDontPlayType(ctx => _hasAttackedLastTurn[memberId] && ctx.Member.State.PrimaryResourceAmount > 0, CardTag.Attack)
            .IfTruePlayType(_ => !_hasAttackedLastTurn[memberId], CardTag.Attack)
            .WithCommonSenseSelections()
            .WithFinalizedCardSelection();
        
        if (card.SelectedCard.Value.Is(CardTag.Ultimate))
            return new PlayedCardV2(
                battleState.Members[memberId], 
                new Target[] { ultimateTarget }, 
                card.SelectedCard.Value.CreateInstance(battleState.GetNextCardId(), battleState.Members[memberId]));
        return card.WithSelectedTargetsPlayedCard();
    }
}