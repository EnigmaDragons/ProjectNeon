using UnityEngine;

public class PlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;

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
        {
            var action = _card.Actions[index];
            if (!string.IsNullOrWhiteSpace(action.CharacterAnimation))
                BattleEvent.Publish(new CharacterAnimationRequested(_performer.Id, action.CharacterAnimation));
            
            if (_targets.Length <= index) 
                Debug.LogError($"Invalid Targets for {_card.Name}. Action {index}");
            action.Apply(_performer, _targets[index]);
        }
    }
}
