using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Card : ScriptableObject
{
    public Sprite Art;
    public List<CardEffect> Effects;
    public string Description;
    public string TypeDescription;
}
