using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckListViewCards : MonoBehaviour
{
    [SerializeField]
    private Party party;

    private List<Card> cards;

    public void Init()
    {
        this.party.characterOne.Cards.ForEach(
            card => this.cards.Add(card)
        );
        this.party.characterTwo.Cards.ForEach(
            card => this.cards.Add(card)
        );
        this.party.characterThree.Cards.ForEach(
            card => this.cards.Add(card)
        );

    }
}
