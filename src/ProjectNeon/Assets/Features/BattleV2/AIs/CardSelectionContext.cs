using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CardSelectionContext
{
    public Member Member { get; }
    public Member[] Enemies => State.GetConsciousEnemies(Member);
    public Member[] Allies => State.GetConsciousAllies(Member);
    public BattleState State { get; }
    public AIStrategy Strategy { get; }
    public IEnumerable<CardTypeData> CardOptions { get; } = new List<CardTypeData>();
    public Maybe<CardTypeData> SelectedCard { get; private set; } = Maybe<CardTypeData>.Missing();
    
    public CardSelectionContext(int memberId, BattleState state, AIStrategy strategy)
        : this(state.Members[memberId], state, strategy, state.GetPlayableCards(memberId)) {}
    
    public CardSelectionContext(Member member, BattleState state, AIStrategy strategy)
        : this(member, state, strategy, state.GetPlayableCards(member.Id)) {}
    
    public CardSelectionContext(Member member, BattleState state, AIStrategy strategy, IEnumerable<CardTypeData> options)
    {
        Member = member;
        State = state;
        Strategy = strategy;
        CardOptions = options;
    }
    
    public CardSelectionContext WithOptions(IEnumerable<CardTypeData> options) 
        => new CardSelectionContext(Member, State, Strategy, options) { SelectedCard = SelectedCard };
    
    public CardSelectionContext IfTrueDontPlayType(Func<CardSelectionContext, bool> shouldRefine, params CardTag[] excludedTagsCombination)
        => shouldRefine(this)
            ? new CardSelectionContext(Member, State, Strategy, CardOptions.Where(o => !o.Is(excludedTagsCombination))) { SelectedCard = SelectedCard }
            : this;
    
    public CardSelectionContext IfTruePlayType(Func<CardSelectionContext, bool> shouldRefine, params CardTag[] includeTagsCombination)
        => shouldRefine(this)
            ? new CardSelectionContext(Member, State, Strategy, CardOptions.Where(o => o.Is(includeTagsCombination))) { SelectedCard = SelectedCard }
            : this;

    public CardSelectionContext WithSelectedCard(CardTypeData card)
        => new CardSelectionContext(Member, State, Strategy, CardOptions) { SelectedCard = new Maybe<CardTypeData>(card)};
}
