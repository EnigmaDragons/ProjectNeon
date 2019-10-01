using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterController;

public class DeckController : MonoBehaviour
{
    [SerializeField] private PartyDecks decks;

    private Deck current;

    public void OnHeroChange()
    {
        this.current = decks.Decks[
            Array.IndexOf(
                Enum.GetValues(CharacterController.currentCharacter.GetType()),
                CharacterController.currentCharacter)
            ];
    }


}
