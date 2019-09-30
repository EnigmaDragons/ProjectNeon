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
     * @todo #207:30min We need a better solution to link targets to effects. Currently we rely on the order of
     *  both arrays: the Nth effect in card.efects will target the Nth element from target[]. This is very dangerous
     *  because we can't guarantee in which particular order they are being stored or created. We need to find a better
     *  way to relate Effect with Target
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
