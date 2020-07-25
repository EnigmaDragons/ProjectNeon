public class PlayedCardV2 : IPlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;

    public PlayedCardV2(Member performer, Target[] targets, Card card)
    {
        _performer = performer;
        _targets = targets;
        _card = card;
    }

    public Member Member => _performer;
    public Card Card => _card;

    public void Perform()
    {
        for (var index = 0; index < _card.BattleActions.Length; index++)
        {
            var action = _card.BattleActions[index];
            if (action.Type == CardBattleActionType.Battle)
                action.Apply(_performer, _targets);
        }
    }
}