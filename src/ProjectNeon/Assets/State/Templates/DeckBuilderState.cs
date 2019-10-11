using System.Collections.Generic;
using UnityEngine;

/**
 * State for Deck Builder scene
 */
public class DeckBuilderState : ScriptableObject
{
    public CharactersEnum currentCharacter = CharactersEnum.Character1;
    [SerializeField] private PartyDecks decks;
    [SerializeField] private Hero currentHero;
    [SerializeField] private GameEvent onCurrentDeckChanged;
    [SerializeField] private Library library;

    [SerializeField] private IntVariable minimumDeckSize;
    public int MinimumDeckSize => this.minimumDeckSize.Value;

    public GameEvent OnCurrentDeckChanged => onCurrentDeckChanged;

    public Hero CurrentHero
    {
        get => currentHero;
        set => currentHero = value;
    }

    private Deck current;
    public Deck Current()
    {
        return this.decks.Decks[(int)currentCharacter];
    }

    public List<Card> GetPossibleCardsForCurrentHero()
    {
        List<Card> cards = new List<Card>();
        library.UnlockedCards.ForEach(
            card =>
            {
                if (
                    card.LimitedToClass.IsPresent &&
                    card.LimitedToClass.Value.Contains(currentHero.name)
                )
                {
                    cards.Add(card);
                }
            }
        );
        return cards;
    }

    public bool HasDeckMinimumSize()
    {
        return (this.Current().Cards.Count >= minimumDeckSize.Value);
    }

    public void ListToDeck(Card card)
    {
        this.decks.Decks[0].Cards.Remove(card);
        this.current.Cards.Add(card);
        onCurrentDeckChanged.Publish();
        /**
         * @todo #216:30min We need to discover which deck belongs to which hero, so we can
         *  select the correct deck to remove or add the cards to. Maybe PartyDecks must be refactored
         *  to a MAP like structure with the heroes as keys. Then fix both ListToDeck and DeckToList
         *  methods to call the correct deck.
         */
    }

    public void DeckToList(Card card)
    {
        this.current.Cards.Remove(card);
        this.decks.Decks[0].Cards.Add(card);
        onCurrentDeckChanged.Publish();
    }

    /**
     * @todo #216:30min We need to correctly initialize and update current Deck. This deck should
     *  change each time we change character in DeckBuilderUi (ChangeCurrentCharacter Event).
     */
}