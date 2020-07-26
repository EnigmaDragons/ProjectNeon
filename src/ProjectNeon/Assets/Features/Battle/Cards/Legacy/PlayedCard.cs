using System;
using UnityEngine;

[Obsolete("Battle V1")]
public class PlayedCard : IPlayedCard
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
    public ResourceQuantity Spent => new ResourceQuantity { Amount = Card.Cost.Amount, ResourceType = Card.Cost.ResourceType };
    public ResourceQuantity Gained => new ResourceQuantity { Amount = 0, ResourceType = Card.Cost.ResourceType};
    
    public void Perform()
    {
        for (var index = 0; index < _card.Actions.Length; index++)
        {
            var action = _card.Actions[index];
            if (!string.IsNullOrWhiteSpace(action.CharacterAnimation))
                Message.Publish(new CharacterAnimationRequested(_performer.Id, action.CharacterAnimation));
            if (!string.IsNullOrWhiteSpace(action.EffectAnimation))
                Message.Publish(new BattleEffectAnimationRequested
                {
                    PerformerId = _performer.Id,
                    EffectName = action.EffectAnimation,
                    Group = action.Group,
                    Scope = action.Scope,
                    Target = _targets[index]
                });
            
            if (_targets.Length <= index) 
                Debug.LogError($"Invalid Targets for {_card.Name}. Action {index}");
            action.Apply(_performer, _targets[index]);
        }
    }
}
