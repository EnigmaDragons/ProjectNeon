using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CardSelectionContext
{
    public Member Member { get; }
    public Member[] Enemies => AllMembers.GetConsciousEnemies(Member);
    public Member[] Allies => AllMembers.GetConsciousAllies(Member);
    public Member[] AllMembers { get; }
    public AIStrategy Strategy { get; }
    public IEnumerable<CardTypeData> CardOptions { get; }
    public PartyAdventureState PartyAdventureState { get; }
    public CardPlayZones Zones { get; }
    
    public Maybe<CardTypeData> SelectedCard { get; private set; } = Maybe<CardTypeData>.Missing();
    
    public CardSelectionContext(int memberId, BattleState state, AIStrategy strategy)
        : this(state.Members[memberId], state, strategy, state.GetPlayableCards(memberId)) {}
    
    public CardSelectionContext(Member member, BattleState state, AIStrategy strategy)
        : this(member, state, strategy, state.GetPlayableCards(member.Id)) {}
    
    public CardSelectionContext(Member member, BattleState state, AIStrategy strategy, IEnumerable<CardTypeData> options)
        : this(member, state.MembersWithoutIds, strategy, state.Party, state.PlayerCardZones, options) {}
    
    public CardSelectionContext(Member member, Member[] allMembers, AIStrategy strategy, PartyAdventureState partyAdventureState, CardPlayZones zones, IEnumerable<CardTypeData> options)
    {
        Member = member;
        AllMembers = allMembers;
        Strategy = strategy;
        PartyAdventureState = partyAdventureState;
        Zones = zones;
        CardOptions = options;
    }
    
    public CardSelectionContext IfTrueDontPlayType(Func<CardSelectionContext, bool> shouldRefine, params CardTag[] excludedTagsCombination)
    {
        if (!excludedTagsCombination.Any())
        {
            Log.Error($"{Member.Name} attempted to exclude all card types");
            return this;
        }
        return shouldRefine(this)
            ? new CardSelectionContext(Member, AllMembers, Strategy, PartyAdventureState, Zones, CardOptions.Where(o => !o.Is(excludedTagsCombination))) { SelectedCard = SelectedCard }
            : this;
    }
    
    public CardSelectionContext IfTruePlayType(Func<CardSelectionContext, bool> shouldRefine, params CardTag[] includeTagsCombination)
        => shouldRefine(this)
            ? new CardSelectionContext(Member, AllMembers, Strategy, PartyAdventureState, Zones, CardOptions.Where(o => o.Is(includeTagsCombination))) { SelectedCard = SelectedCard }
            : this;

    public CardSelectionContext WithSelectedCard(CardTypeData card)
        => new CardSelectionContext(Member, AllMembers, Strategy, PartyAdventureState, Zones, CardOptions) { SelectedCard = new Maybe<CardTypeData>(card)};
}
