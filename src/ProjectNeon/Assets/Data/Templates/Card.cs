using System;
using System.Linq;
using UnityEngine;

public class Card : ScriptableObject
{
    [SerializeField] private Sprite art;
    [SerializeField] private string description;
    [SerializeField] private string typeDescription;
    [SerializeField] private StringVariable onlyPlayableByClass;
    [SerializeField] private CardAction cardAction1;
    [SerializeField] private CardAction cardAction2;

    // @todo #1:15min Add Resource Cost to Card
    
    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => typeDescription;
    public Maybe<string> LimitedToClass => new Maybe<string>(onlyPlayableByClass.Value.Length > 0 ? onlyPlayableByClass.Value : null);
    
    public CardAction[] Actions => Array.Empty<CardAction>()
        .ConcatIf(cardAction1, c => c.HasEffects)
        .ConcatIf(cardAction2, c => c.HasEffects)
        .ToArray();
}
