using System.Collections.Generic;

public sealed class CardSelectionContext
{
    public Member Member { get; }
    public BattleState State { get; }
    public AIStrategy Strategy { get; }
    public IEnumerable<CardTypeData> CardOptions { get; private set; } = new List<CardTypeData>();
    public Maybe<CardTypeData> SelectedCard { get; private set; } = Maybe<CardTypeData>.Missing();
    
    public CardSelectionContext(Member member, BattleState state, AIStrategy strategy)
    {
        Member = member;
        State = state;
        Strategy = strategy;
    }
    
    public CardSelectionContext WithOptions(IEnumerable<CardTypeData> options) 
        => new CardSelectionContext(Member, State, Strategy) { CardOptions = options, SelectedCard = SelectedCard };
    
    public CardSelectionContext withSelectedCard(CardTypeData card)
        => new CardSelectionContext(Member, State, Strategy) { CardOptions = CardOptions, SelectedCard = new Maybe<CardTypeData>(card)};
}
