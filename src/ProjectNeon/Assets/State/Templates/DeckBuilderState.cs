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


    /**
     * @todo #216:30min We need to correctly initialize and update current Deck. This deck should
     *  change each time we change character in DeckBuilderUi (ChangeCurrentCharacter Event).
     */
}