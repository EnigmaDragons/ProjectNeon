using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{
    public Sprite Art;
    public string Description;
    public string TypeDescription;

    [SerializeField] private List<CardAction<Target>> actions;
}
