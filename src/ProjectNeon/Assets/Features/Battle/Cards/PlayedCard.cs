
public class PlayedCard
{
    private Card _card;
    private Member _performer;

    /**
     * Each Target targeted by the Card effects in the same order that the effects are stored in Card.
     * 
     * @todo #207:30min We need a better solution to link targets to effects. Currently we rely on the order of
     *  both arrays: the Nth effect in card.efects will target the Nth element from target[]. This is very dangerous
     *  because we can't guarantee in which particular order they are being stored or created. We need to find a better
     *  way to relate Effect with Target
     */ 
    private Target[] _targets;

    public PlayedCard(Member performer, Target[] targets, Card card)
    {
        _performer = performer;
        _targets = targets;
        _card = card;
    }

    public void Perform()
    {
        int index = 0;
        _card.Actions.ForEach(
            action => 
            {
                action.Apply(_performer, _targets[index]);
                index++;
            }
        );
    }
}
