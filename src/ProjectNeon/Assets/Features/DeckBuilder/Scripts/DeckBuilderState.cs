using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * State for Deck Builder scene
 */
public class DeckBuilderState : ScriptableObject
{

    [SerializeField] private PartyDecks decks;

    [SerializeField] private GameEvent onCurrentDeckChanged;
    public GameEvent OnCurrentDeckChanged => onCurrentDeckChanged;

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
