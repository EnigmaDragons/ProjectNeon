﻿using System.Linq;
using UnityEngine;

public class Card : ScriptableObject
{
    [SerializeField] private Sprite art;
    [SerializeField] private string description;
    [SerializeField] private string typeDescription;
    [SerializeField] private CardAction[] actions;

    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => typeDescription;
    public CardAction[] Actions => actions.ToArray();

}
