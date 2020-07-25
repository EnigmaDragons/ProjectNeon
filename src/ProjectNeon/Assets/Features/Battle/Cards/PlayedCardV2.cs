public class PlayedCardV2 : IPlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;
    private readonly ResourcesSpent _spent;

    public PlayedCardV2(Member performer, Target[] targets, Card card, ResourcesSpent spent)
    {
        _performer = performer;
        _targets = targets;
        _card = card;
        _spent = spent;
    }

    public Member Member => _performer;
    public Card Card => _card;
    public ResourcesSpent Spent => _spent;

    public void Perform()
    {
        Message.Subscribe<Finished<ApplyBattleEffect>>(_ => Continue(), this);
        Message.Subscribe<Finished<CharacterAnimationRequested>>(_ => Continue(), this);
        Message.Subscribe<Finished<BattleEffectAnimationRequested>>(_ => Continue(), this);
        _sequenceIndex = 0;
        _actionIndex = 0;
        Continue();
    }

    private int _sequenceIndex;
    private int _actionIndex;

    public void Continue()
    {
        if (_card.ActionSequences.Length == _sequenceIndex 
            || (_card.ActionSequences.Length - 1 == _sequenceIndex && _card.ActionSequences[_sequenceIndex].CardActions.Length == _actionIndex))
        {
            Message.Unsubscribe(this);
            Message.Publish(new CardResolutionFinished());
            return;
        }

        var seq = _card.ActionSequences[_sequenceIndex];
        var action = seq.CardActions[_actionIndex];
        action.Begin(_performer, _targets[_sequenceIndex], seq.Group, seq.Scope, _spent.Amount);
        _actionIndex++;
        
        if (seq.CardActions.Length == _actionIndex)
        {
            _sequenceIndex++;
            _actionIndex = 0;
        }
    }
}