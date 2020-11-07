using System;
using UnityEngine;

[Serializable]
public class CardActionSequence
{
    [SerializeField] private Scope scope;
    [SerializeField] private Group group;
    [SerializeField] private AvoidanceType avoidance;
    [SerializeField] public CardActionsData cardActions;

    public Scope Scope => scope;
    public Group Group => group;
    public AvoidanceType AvoidanceType => avoidance;
    public CardActionsData CardActions => cardActions;
    
    public static CardActionSequence ForReaction(CardActionsData d)
    {
        var c = new CardActionSequence {cardActions = d};
        return c;
    }
}
