using UnityEngine;

/**
 * Represents a played card from enemy or player character.
 */
 
public class PlayedCard : MonoBehaviour
{
    /**
     * Card played
     */
    private Card card;

    /**
     * Member which played the card
     */
    private Member performer;

    /**
     * Each Target targeted by the Card effects in the same order that the effects are stored in Card.
     * 
     * @todo #207:30min We need a better solution to link targets to effects.
     */ 
    private Target[] target;

    public PlayedCard Init(Member performer, Target[] target, Card card)
    {
        this.performer = performer;
        this.target = target;
        this.card = card;
        return this;
    }
}
