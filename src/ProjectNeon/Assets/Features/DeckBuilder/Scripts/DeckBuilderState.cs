using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * State for Deck Builder scene
 */
public class DeckBuilderState : ScriptableObject
{
    public CharactersEnum currentCharacter = CharactersEnum.Character1;
    [SerializeField] private PartyDecks decks;
    [SerializeField] private GameEvent onCurrentDeckChanged;
    public GameEvent OnCurrentDeckChanged => onCurrentDeckChanged;

    private Deck current;
    public Deck Current()
    {
        return this.decks.Decks[(int)currentCharacter];
    }
}
