using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * State for Deck Builder scene
 */
public class DeckBuilderState : ScriptableObject
{
    /**
     * Party Decks
     */
    [SerializeField] private PartyDecks decks;

    /**
     * Current deck changed event
     */
    [SerializeField] private GameEvent onCurrentDeckChanged;
    public GameEvent OnCurrentDeckChanged => onCurrentDeckChanged;

    /**
     * Current deck from the hero selected in Deck Builder Scene
     */
    private Deck current;
    public Deck Current()
    {
        return this.decks.Decks[0];
    }

    /**
     * @todo #216:30min We need to correctly initialize and update current Deck. This deck should
     *  change each time we change character in DeckBuilderUi (ChangeCurrentCharacter Event).
     */
}
