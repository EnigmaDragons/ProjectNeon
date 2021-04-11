using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStageDetails
{
    //change these back to private at some point
    [SerializeField] public int stage;
    [SerializeField] public int powerLevel;
    [SerializeField] public int maxHp;
    [SerializeField] public int maxShield;
    [SerializeField] public int startingShield;
    [SerializeField] public int toughness;
    [SerializeField] public int attack;
    [SerializeField] public int magic;
    [SerializeField] public int leadership;
    [SerializeField] public float armor;
    [SerializeField] public float resistance;
    [SerializeField] public float nonStatCardValueFactor;
    [SerializeField] public int startingResourceAmount = 0;
    [SerializeField] public int maxResourceAmount = 0;
    [SerializeField] public int resourceGainPerTurn = 1;
    [SerializeField] public int cardsPerTurn = 1;
    [SerializeField] public EffectData[] startOfBattleEffects = new EffectData[0];
    [SerializeField] public List<CardType> Cards = new List<CardType>();
}