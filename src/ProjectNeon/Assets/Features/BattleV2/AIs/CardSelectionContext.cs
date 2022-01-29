using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CardSelectionContext
{
    public Member Member { get; }
    public Maybe<Member> FocusTarget { get; }
    public Member[] Enemies => AllMembers.GetConsciousEnemies(Member);
    public Member[] Allies => AllMembers.GetConsciousAllies(Member);
    public Member[] AllMembers { get; }
    public AIStrategy Strategy { get; }
    public IEnumerable<CardTypeData> CardOptions { get; }
    public PartyAdventureState PartyAdventureState { get; }
    public CardPlayZones Zones { get; }
    public AiPreferences AiPreferences { get; }
    public EnemySpecialCircumstanceCards SpecialCards => Strategy.SpecialCards;
    
    public Maybe<CardTypeData> SelectedCard { get; private set; } = Maybe<CardTypeData>.Missing();
    public string CardOptionsString => $"[{string.Join(", ", CardOptions.Select(x => x.Name))}]";

    public CardSelectionContext(int memberId, BattleState state, AIStrategy strategy)
        : this(state.Members.VerboseGetValue(memberId, "Members"), Maybe<Member>.Missing(), state.MembersWithoutIds,
            strategy, state.Party, state.PlayerCardZones, state.GetAiPreferences(memberId), 
            state.GetPlayableCards(memberId, state.Party, strategy.SpecialCards)) {}
    
    public CardSelectionContext(Member member, BattleState state, AIStrategy strategy, Maybe<Member> focusTarget)
        : this(member, focusTarget, state.MembersWithoutIds, strategy, state.Party, state.PlayerCardZones, 
            state.GetAiPreferences(member.Id), state.GetPlayableCards(member.Id, state.Party, strategy.SpecialCards)) {}

    private CardSelectionContext(Member member, Maybe<Member> focusTarget, Member[] allMembers, AIStrategy strategy, 
        PartyAdventureState partyAdventureState, CardPlayZones zones, AiPreferences aiPreferences, IEnumerable<CardTypeData> options)
    {
        Member = member;
        FocusTarget = focusTarget;
        AllMembers = allMembers;
        Strategy = strategy;
        PartyAdventureState = partyAdventureState;
        Zones = zones;
        CardOptions = options;
        AiPreferences = aiPreferences;
    }
    
    public CardSelectionContext IfTrueDontPlayType(Func<CardSelectionContext, bool> shouldRefine, params CardTag[] excludedTagsCombination)
    {
        if (!excludedTagsCombination.Any())
        {
            Log.Error($"{Member.Name} attempted to exclude all card types");
            return this;
        }
        return LogAfter(() => shouldRefine(this)
            ? new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, CardOptions.Where(o => !o.Is(excludedTagsCombination))) { SelectedCard = SelectedCard }
            : this);
    }
    
    public CardSelectionContext IfTrueDontPlay(Func<CardSelectionContext, bool> shouldRefine, Func<CardTypeData, bool> isExcludedCard) =>
        LogAfter(() => shouldRefine(this)
            ? new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, CardOptions.Where(c => !isExcludedCard(c))) { SelectedCard = SelectedCard }
            : this);

    public CardSelectionContext IfTruePlayType(Func<CardSelectionContext, bool> shouldRefine, params CardTag[] includeTagsCombination) => 
        LogAfter(() => shouldRefine(this)
            ? new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, CardOptions.Where(o => o.Is(includeTagsCombination))) { SelectedCard = SelectedCard }
            : this);

    public CardSelectionContext WithSelectedCard(CardTypeData card)
        => new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, CardOptions) { SelectedCard = new Maybe<CardTypeData>(card)};

    public CardSelectionContext WithCardOptions(IEnumerable<CardTypeData> options)
        => new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, options) { SelectedCard = SelectedCard };

    private CardSelectionContext LogAfter(Func<CardSelectionContext> getNext)
    {
        var ctx = getNext();
        Log.Info($"AI - {ctx.Member.Name} - {ctx.SelectedCard.Select(x => $"Selected {x.Name}", () => "Not Selected Yet")} - Options {ctx.CardOptionsString}");
        return ctx;
    }
}
