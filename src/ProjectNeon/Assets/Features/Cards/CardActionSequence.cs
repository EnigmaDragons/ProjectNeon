using System;
using UnityEngine;

[Serializable]
public class CardActionSequence
{
    [SerializeField] private Scope scope;
    [SerializeField] private Group group;
    [SerializeField] public CardActionsData cardActions;

    public Scope Scope => scope;
    public Group Group => group;
    public CardActionsData CardActions => cardActions;

    public void Play(Member source, Target target, Group group, Scope scope, int amountPaid)
        => cardActions.Play(source, target, group, scope, amountPaid);
}