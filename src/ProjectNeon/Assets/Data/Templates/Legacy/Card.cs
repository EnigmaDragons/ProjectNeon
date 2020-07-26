using System;
using System.Linq;
using UnityEngine;

public class Card : ScriptableObject
{
    [PreviewSprite] [SerializeField] private Sprite art;
    [SerializeField] private string description;
    [SerializeField] private StringVariable typeDescription;
    [SerializeField] private StringVariable onlyPlayableByClass;
    [SerializeField] private ResourceCost cost;
    [SerializeField] public CardActionSequence[] actionSequences;
    [SerializeField] private CardAction cardAction1;
    [SerializeField] private CardAction cardAction2;

    public string Name => name.SkipThroughFirstDash().WithSpaceBetweenWords();
    public ResourceCost Cost => cost;
    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => typeDescription.Value;
    public Maybe<string> LimitedToClass => new Maybe<string>(onlyPlayableByClass.Value.Length > 0 ? onlyPlayableByClass.Value : null);
    public CardActionSequence[] ActionSequences => actionSequences == null ? new CardActionSequence[0] : actionSequences.ToArray();

    public CardAction[] Actions => Array.Empty<CardAction>()
        .ConcatIf(cardAction1, c => c.HasEffects)
        .ConcatIf(cardAction2, c => c.HasEffects)
        .ToArray();

    public void Play(Member source, Target target, Group group, Scope scope, int amountPaid)
        => ActionSequences.ForEach(x => x.Play(source, target, group, scope, amountPaid));
}
