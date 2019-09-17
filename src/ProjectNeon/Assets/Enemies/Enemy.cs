using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Enemy : ScriptableObject
{
    [SerializeField]
    private string enemyName;

    [SerializeField]
    private Deck deck;

    [SerializeField]
    private TurnAI turn;

    [SerializeField]
    private Stats stats;

    public Sprite image;

}
