using System.Linq;

public class MagiChemistAI : TurnAI
{
    private bool _hasAttackedLastTurn;
    
    public override void InitForBattle()
    {
        _hasAttackedLastTurn = Rng.Bool();
    }
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var card = new CardSelectionContext(memberId, battleState, strategy)
            .IfTrueDontPlayType(ctx => ctx.Member.CurrentHp() == ctx.Member.MaxHp() || ctx.Enemies.Any(e => e.CurrentHp() >= (ctx.Member.CurrentHp() + 15)), CardTag.Ultimate)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .IfTrueDontPlayType(_ => _hasAttackedLastTurn, CardTag.Attack)
            .IfTruePlayType(_ => !_hasAttackedLastTurn, CardTag.Attack)
            .WithCommonSenseSelections()
            .WithFinalizedCardSelection();
        _hasAttackedLastTurn = card.SelectedCard.Value.Is(CardTag.Attack);
        if (card.SelectedCard.Value.Is(CardTag.Ultimate))
            return new PlayedCardV2(
                battleState.Members[memberId], 
                new Target[] { new Single(battleState.Enemies.Shuffled().OrderByDescending(x => x.CurrentHp()).First()) }, 
                card.SelectedCard.Value.CreateInstance(battleState.GetNextCardId(), battleState.Members[memberId]));
        return card.WithSelectedTargetsPlayedCard();
    }
}