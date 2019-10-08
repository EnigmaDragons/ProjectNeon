using System.Collections.Generic;
using UnityEngine;

/**
 * State for Deck Builder scene
 */
public class DeckBuilderState : ScriptableObject
{
    [SerializeField] private PartyDecks decks;
    [SerializeField] private Hero currentHero;
    [SerializeField] private GameEvent onCurrentDeckChanged;
    [SerializeField] private Library _library;

    public GameEvent OnCurrentDeckChanged => onCurrentDeckChanged;

    public Hero CurrentHero
    {
        get => currentHero;
        set => currentHero = value;
    }

    private Deck current;
    public Deck Current()
    {
        Deck deck = new Deck();
        _library.UnlockedCards.ForEach(
            card =>
            {
                if (
                    card.LimitedToClass.IsPresent &&
                    card.LimitedToClass.Value.Contains(currentHero.name)
                )
                {
                    deck.Add(card);
                }
            }
        );
        return deck;
    }

    /**
     * @todo #216:30min We need to correctly initialize and update current Deck. This deck should
     *  change each time we change character in DeckBuilderUi (ChangeCurrentCharacter Event).
     */
}
