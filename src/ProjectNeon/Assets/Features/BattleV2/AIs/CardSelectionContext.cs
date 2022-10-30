using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CardSelectionContext
{
    public Member Member { get; }
    public Maybe<Member> FocusTarget { get; }
    public Member[] Enemies => AllMembers.GetConsciousEnemies(Member);
    public Member[] Allies => AllMembers.GetConsciousAllies(Member);
    public Member[] NonSelfAllies => Allies.Except(Member).ToArray();
    public Member[] AllMembers { get; }
    public AIStrategy Strategy { get; }
    public IEnumerable<CardTypeData> CardOptions { get; }
    public HashSet<CardTypeData> UnhighlightedCardOptions { get; }
    public PartyAdventureState PartyAdventureState { get; }
    public CardPlayZones Zones { get; }
    public AiPreferences AiPreferences { get; }
    public Maybe<CardTypeData> LastPlayedCard { get; }

    public EnemySpecialCircumstanceCards SpecialCards => Strategy.SpecialCards;
    
    public Maybe<CardTypeData> SelectedCard { get; private set; } = Maybe<CardTypeData>.Missing();
    public CardTypeData[] CurrentTurnPlayedCards { get; private set; } = Array.Empty<CardTypeData>();
    public string CardOptionsString => $"[{string.Join(", ", CardOptions.Select(x => x.Name))}]";
    public int MemberNumberOfCardsPlays => Member.ExtraCardPlays();

    public CardSelectionContext(int memberId, BattleState state, AIStrategy strategy)
        : this(state.Members.VerboseGetValue(memberId, "Members"), Maybe<Member>.Missing(), state.MembersWithoutIds,
            strategy, state.Party, state.PlayerCardZones, state.GetAiPreferences(memberId), Maybe<CardTypeData>.Missing(),
            state.GetPlayableCards(memberId, state.Party, strategy.SpecialCards), state.GetUnhighlightedCards(memberId, state.Party, strategy.SpecialCards)) {}
    
    public CardSelectionContext(Member member, BattleState state, AIStrategy strategy, Maybe<Member> focusTarget, 
        Maybe<CardTypeData> lastPlayedCard, IEnumerable<CardTypeData> unhighlightedCardOptions)
        : this(member, focusTarget, state.MembersWithoutIds, strategy, state.Party, state.PlayerCardZones, 
            state.GetAiPreferences(member.Id), lastPlayedCard, state.GetPlayableCards(member.Id, state.Party, strategy.SpecialCards), unhighlightedCardOptions) {}

    public CardSelectionContext(Member member, Maybe<Member> focusTarget, Member[] allMembers, AIStrategy strategy, 
        PartyAdventureState partyAdventureState, CardPlayZones zones, AiPreferences aiPreferences, Maybe<CardTypeData> lastPlayedCard,
        IEnumerable<CardTypeData> options, IEnumerable<CardTypeData> unhighlightedCardOptions)
    {
        Member = member;
        FocusTarget = focusTarget;
        AllMembers = allMembers;
        Strategy = strategy;
        PartyAdventureState = partyAdventureState;
        Zones = zones;
        CardOptions = options;
        UnhighlightedCardOptions = unhighlightedCardOptions.ToHashSet();
        LastPlayedCard = lastPlayedCard;
        AiPreferences = aiPreferences;
    }
    
    public CardSelectionContext IfTrueDontPlayType(Func<CardSelectionContext, bool> shouldRefine, params CardTag[] excludedTagsCombination)
    {
        if (!excludedTagsCombination.Any())
        {
            Log.Error($"{Member.NameTerm.ToEnglish()} attempted to exclude all card types");
            return this;
        }
        return LogAfter(() => shouldRefine(this)
            ? new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, 
                LastPlayedCard, CardOptions.Where(o => !o.Is(excludedTagsCombination)), UnhighlightedCardOptions) 
                    { SelectedCard = SelectedCard, CurrentTurnPlayedCards = CurrentTurnPlayedCards }
            : this);
    }
    
    public CardSelectionContext IfTrueDontPlay(Func<CardSelectionContext, bool> shouldRefine, Func<CardTypeData, bool> isExcludedCard) =>
        LogAfter(() => shouldRefine(this)
            ? new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, 
                LastPlayedCard, CardOptions.Where(c => !isExcludedCard(c)), UnhighlightedCardOptions) 
                    { SelectedCard = SelectedCard, CurrentTurnPlayedCards = CurrentTurnPlayedCards }
            : this);

    public CardSelectionContext IfTruePlayType(Func<CardSelectionContext, bool> shouldRefine, params CardTag[] includeTagsCombination) => 
        LogAfter(() => shouldRefine(this)
            ? new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, LastPlayedCard, 
                CardOptions.Where(o => o.Is(includeTagsCombination)), UnhighlightedCardOptions) 
                    { SelectedCard = SelectedCard, CurrentTurnPlayedCards = CurrentTurnPlayedCards }
            : this);

    public CardSelectionContext WithSelectedCard(CardTypeData card)
        => new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, 
            LastPlayedCard, CardOptions, UnhighlightedCardOptions) 
                { SelectedCard = new Maybe<CardTypeData>(card), CurrentTurnPlayedCards = CurrentTurnPlayedCards };

    public CardSelectionContext WithCardOptions(IEnumerable<CardTypeData> options)
        => new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, 
            LastPlayedCard, options, UnhighlightedCardOptions) 
                { SelectedCard = SelectedCard, CurrentTurnPlayedCards = CurrentTurnPlayedCards };
    
    public CardSelectionContext WithLastPlayedCard(CardTypeData lastPlayedCard)
        => WithLastPlayedCard(Maybe<CardTypeData>.Present(lastPlayedCard));
    
    public CardSelectionContext WithLastPlayedCard(Maybe<CardTypeData> lastPlayedCard)
        => new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, 
            lastPlayedCard, CardOptions, UnhighlightedCardOptions) 
                { SelectedCard = SelectedCard, CurrentTurnPlayedCards = CurrentTurnPlayedCards};
    
    public CardSelectionContext WithCurrentTurnPlayedCards(CardTypeData[] currentTurnPlayedCards)
        => new CardSelectionContext(Member, FocusTarget, AllMembers, Strategy, PartyAdventureState, Zones, AiPreferences, 
            LastPlayedCard, CardOptions, UnhighlightedCardOptions) 
                { SelectedCard = SelectedCard, CurrentTurnPlayedCards = currentTurnPlayedCards };

    private CardSelectionContext LogAfter(Func<CardSelectionContext> getNext)
    {
        var ctx = getNext();
        Log.Info($"AI - {ctx.Member.NameTerm.ToEnglish()} - {ctx.SelectedCard.Select(x => $"Selected {x.Name}", () => "Not Selected Yet")} - Options {ctx.CardOptionsString}");
        return ctx;
    }
}
