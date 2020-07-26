public class PlayedCardV2 : IPlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;
    private readonly ResourceQuantity _spent;
    private readonly ResourceQuantity _gained;

    public PlayedCardV2(Member performer, Target[] targets, Card card)
        : this(performer, targets, card, card.ResourcesSpent(performer), card.ResourcesGained(performer)) {}
    public PlayedCardV2(Member performer, Target[] targets, Card card, ResourceQuantity spent, ResourceQuantity gained)
    {
        _performer = performer;
        _targets = targets;
        _card = card;
        _spent = spent;
        _gained = gained;
    }

    public Member Member => _performer;
    public Card Card => _card;
    public ResourceQuantity Spent => _spent;
    public ResourceQuantity Gained => _gained;

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
            || (_card.ActionSequences.Length - 1 == _sequenceIndex && _card.ActionSequences[_sequenceIndex].CardActions.Actions.Length == _actionIndex))
        {
            Message.Unsubscribe(this);
            Message.Publish(new CardResolutionFinished());
            return;
        }

        var seq = _card.ActionSequences[_sequenceIndex];
        var action = seq.CardActions.Actions[_actionIndex];
        action.Begin(_performer, _targets[_sequenceIndex], seq.Group, seq.Scope, _spent.Amount);
        _actionIndex++;
        
        if (seq.CardActions.Actions.Length == _actionIndex)
        {
            _sequenceIndex++;
            _actionIndex = 0;
        }
    }
}