using System;
using UnityEngine;

[Serializable]
public class CardActionSequence
{
    [SerializeField] private Scope scope;
    [SerializeField] private Group group;
    [SerializeField] private CardActionV2[] cardActions;

    public Scope Scope => scope;
    public Group Group => group;
    public CardActionV2[] CardActions => cardActions;
}