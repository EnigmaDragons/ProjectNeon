﻿using System;
using System.Linq;
using UnityEngine;

public class Card : ScriptableObject
{
    [SerializeField] private Sprite art;
    [SerializeField] private string description;
    [SerializeField] private StringVariable typeDescription;
    [SerializeField] private StringVariable onlyPlayableByClass;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private CardAction cardAction1;
    [SerializeField] private CardAction cardAction2;

    public string Name => name.SkipThroughFirstDash().WithSpaceBetweenWords();
    public ResourceCost Cost => cost;
    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => typeDescription.Value;
    public Maybe<string> LimitedToClass => new Maybe<string>(onlyPlayableByClass.Value.Length > 0 ? onlyPlayableByClass.Value : null);
    
    public CardAction[] Actions => Array.Empty<CardAction>()
        .ConcatIf(cardAction1, c => c.HasEffects)
        .ConcatIf(cardAction2, c => c.HasEffects)
        .ToArray();
}
