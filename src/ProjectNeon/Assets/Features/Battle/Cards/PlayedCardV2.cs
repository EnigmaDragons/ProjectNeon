﻿public class PlayedCardV2
{
    private readonly CardV2 _card;
    private readonly Member _performer;
    private readonly Target[] _targets;

    public PlayedCardV2(Member performer, Target[] targets, CardV2 card)
    {
        _performer = performer;
        _targets = targets;
        _card = card;
    }

    public Member Member => _performer;
    public CardV2 Card => _card;

    public void Perform()
    {
        for (var index = 0; index < _card.Actions.Length; index++)
        {
            var action = _card.Actions[index];
            if (action.Type == CardActionType.Battle)
                action.Apply(_performer, _targets);
        }
    }
}