using System;
using UnityEngine;

[Serializable]
public class CardActionSequence
{
    [SerializeField] private Scope scope;
    [SerializeField] private Group group;
    [SerializeField] private AvoidanceType avoidance;
    [SerializeField] public CardActionsData cardActions;
    [SerializeField] private bool repeatX;

    public Scope Scope => scope;
    public Group Group => group;
    public AvoidanceType AvoidanceType => avoidance;
    public CardActionsData CardActions => cardActions;
    public bool RepeatX => repeatX;

    public static CardActionSequence Create(Scope s, Group g, AvoidanceType a, CardActionsData data, bool x)
    {
        return new CardActionSequence
        {
            scope = s,
            @group = g,
            avoidance = a,
            cardActions = data,
            repeatX = x
        };
    }
    
    public static CardActionSequence ForReaction(CardActionsData d)
    {
        var c = new CardActionSequence {cardActions = d};
        return c;
    }
}
