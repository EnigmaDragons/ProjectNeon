using System;
using System.Linq;
using UnityEngine;

public class Card : ScriptableObject
{
    [SerializeField] private Sprite art;
    [SerializeField] private string description;
    [SerializeField] private string typeDescription;
    [SerializeField] private StringVariable onlyPlayableByClass;
    [SerializeField] private CardAction[] actions;

    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => typeDescription;
    public CardAction[] Actions => (actions ?? Array.Empty<CardAction>()).ToArray();
    public Maybe<string> LimitedToClass => new Maybe<string>(onlyPlayableByClass.Value.Length > 0 ? onlyPlayableByClass.Value : null);

}
