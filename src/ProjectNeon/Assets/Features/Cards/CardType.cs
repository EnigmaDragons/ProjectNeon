using System;
using System.Linq;
using UnityEngine;

public class CardType : ScriptableObject
{
    [PreviewSprite] [SerializeField] private Sprite art;
    [SerializeField] [TextArea(1, 12)] private string description;
    [SerializeField] private StringVariable typeDescription;
    [SerializeField] private CharacterClass onlyPlayableByClass;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private ResourceCost onPlayGain;
    [SerializeField] public CardActionSequence[] actionSequences = new CardActionSequence[0];
    [SerializeField] private CardAction cardAction1;
    [SerializeField] private CardAction cardAction2;

    public string Name => name.SkipThroughFirstDash().WithSpaceBetweenWords();
    public ResourceCost Cost => cost;
    public ResourceCost Gain => onPlayGain;
    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => typeDescription.Value;
    public Maybe<CharacterClass> LimitedToClass => new Maybe<CharacterClass>(onlyPlayableByClass != null ? onlyPlayableByClass : null);
    public CardActionSequence[] ActionSequences => actionSequences == null ? new CardActionSequence[0] : actionSequences.ToArray();

    public Card CreateInstance(int id, Member owner) => new Card(id, owner, this);
    
    [Obsolete] public CardAction[] Actions => Array.Empty<CardAction>()
        .ConcatIf(cardAction1, c => c.HasEffects)
        .ConcatIf(cardAction2, c => c.HasEffects)
        .ToArray();

    public void Play(Member source, Target[] targets, int amountPaid)
    {
        if (ActionSequences.Length > targets.Length)
            Debug.LogError($"{Name}: For {ActionSequences.Length} there are only {targets.Length} targets");
        
        for (var i = 0; i < ActionSequences.Length; i++)
            ActionSequences[i].Play(source, targets[i], amountPaid);
    }
}
