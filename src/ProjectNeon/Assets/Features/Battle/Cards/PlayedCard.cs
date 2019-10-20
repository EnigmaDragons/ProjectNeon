
public class PlayedCard
{
    private Card _card;
    private Member _performer;

    private Target[] _targets;

    public PlayedCard(Member performer, Target[] targets, Card card)
    {
        _performer = performer;
        _targets = targets;
        _card = card;
    }

    public Member Member => _performer;
    public Card Card => _card;
    
    public void Perform()
    {
        for (var index = 0; index < _card.Actions.Length; index++)
            _card.Actions[index].Apply(_performer, _targets[index]);
    }
}
