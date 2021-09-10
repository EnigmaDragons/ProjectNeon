using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStageDetails
{
    public int stage;
    public int powerLevel;
    public int maxHp;
    public int maxShield;
    public int startingShield;
    public int startingAegis;
    public int startingPhase = 0;
    public int startingDodge;
    public int startingTaunt;
    public int attack;
    public int magic;
    public int leadership;
    public float armor;
    public float resistance;
    public float nonStatCardValueFactor;
    public int startingResourceAmount = 0;
    public int maxResourceAmount = 0;
    public int resourceGainPerTurn = 1;
    public int cardsPerTurn = 1;
    public EffectData[] startOfBattleEffects = new EffectData[0];
    public List<CardType> Cards = new List<CardType>();

    public Dictionary<string, int> CounterAdjustments => new Dictionary<string, int>
    {
        {TemporalStatType.Aegis.ToString(), startingAegis},
        {TemporalStatType.Dodge.ToString(), startingDodge},
        {TemporalStatType.Phase.ToString(), startingPhase},
        {TemporalStatType.Taunt.ToString(), startingTaunt},
    };
}