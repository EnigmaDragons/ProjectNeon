using System;
using UnityEngine;

[Serializable]
public class EnemyStageDetails
{
    [SerializeField] private int stage;
    [SerializeField] private int powerLevel;
    [SerializeField] private int maxHp;
    [SerializeField] private int maxShield;
    [SerializeField] private int startingShield;
    [SerializeField] private int toughness;
    [SerializeField] private int attack;
    [SerializeField] private int magic;
    [SerializeField] private int leadership;
    [SerializeField] private float armor;
    [SerializeField] private float resistance;
    [SerializeField] private float nonStatCardValueFactor;
    [SerializeField] private int startingResourceAmount = 0;
    [SerializeField] private int maxResourceAmount = 0;
    [SerializeField] private int resourceGainPerTurn = 1;
    [SerializeField] private int cardsPerTurn = 1;
    [SerializeField] private EffectData[] startOfBattleEffects = new EffectData[0];
}