using System.Collections.Generic;
using UnityEngine;

public class Character : ScriptableObject
{
    public Sprite Bust;
    public Stats Stats;
    public string ClassName;

    [SerializeField]
    private List<Card> cards;

    public List<Card> Cards => cards;
}
