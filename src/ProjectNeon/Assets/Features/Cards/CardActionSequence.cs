using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardActionSequence
{
    [SerializeField] private Scope scope;
    [SerializeField] private Group group;
    [SerializeField] public CardActionsData cardActions;
    [SerializeField] private bool repeatX;

    public Scope Scope => scope;
    public Group Group => group;
    public CardActionsData CardActions => cardActions;
    public bool RepeatX => repeatX;
    
    public static CardActionSequence Create(Scope s, Group g, CardActionsData data, bool repeatX)
    {
        return new CardActionSequence
        {
            scope = s,
            @group = g,
            cardActions = data,
            repeatX = repeatX
        };
    }
    
    public static CardActionSequence ForReaction(CardActionsData d)
    {
        var c = new CardActionSequence {cardActions = d};
        return c;
    }
}
