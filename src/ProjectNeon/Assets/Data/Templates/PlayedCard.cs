using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayedCard : MonoBehaviour
{
    private Member performer;
    private Target target;
    private Card card;

    public PlayedCard Init(Member performer, Target target, Card card)
    {
        this.performer = performer;
        this.target = target;
        this.card = card;
        return this;
    }
}
