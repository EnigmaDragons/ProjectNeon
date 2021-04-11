using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private string lastBalanceDate = "Never";
    [SerializeField] private bool isCurrentlyWorking = true;
    [SerializeField] private Deck deck;
    [SerializeField] private TurnAI ai;
    [SerializeField] private int preferredTurnOrder = 99;
    [SerializeField] private int powerLevel = 1;
    [SerializeField] private GameObject prefab;
    [SerializeField] private StringReference deathEffect;
    [SerializeField] private BattleRole battleRole;
    [SerializeField] private EnemyTier tier; 
    [SerializeField] private bool unique;
    [SerializeField] private bool isHasty;
    
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
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int startingResourceAmount = 0;
    [SerializeField] private int maxResourceAmount = 0;
    [SerializeField] private int resourceGainPerTurn = 1;
    [SerializeField] private int cardsPerTurn = 1;
    [SerializeField] private EffectData[] startOfBattleEffects = new EffectData[0];

    [SerializeField] private EnemyStageDetails[] stageDetails = new EnemyStageDetails[0];

    public EnemyInstance GetEnemy(int stage)
    {
        var detail = stageDetails.OrderBy(x => x.stage > stage ? Math.Abs(x.stage - stage) * 2 + 1 : Math.Abs(x.stage - stage) * 2).FirstOrDefault();
        if (detail == null)
            Log.Error($"Enemy {enemyName} has no stage details and can not be used");
        return new EnemyInstance(resourceType, detail.startOfBattleEffects, detail.startingResourceAmount, detail.resourceGainPerTurn, detail.maxResourceAmount, detail.maxHp, detail.maxShield, detail.startingShield, detail.toughness, detail.attack, detail.magic, detail.leadership, detail.armor, detail.resistance, detail.cardsPerTurn, prefab, ai, detail.Cards, battleRole, tier, detail.powerLevel, preferredTurnOrder, enemyName, deathEffect, isHasty, unique);
    } 
    public EffectData[] Effects => startOfBattleEffects;

    public void CopyDataToNewForm()
    {
        if (stageDetails.Any())
            return;
        stageDetails = new[]
        {
            new EnemyStageDetails
            {
                stage = 1,
                powerLevel = powerLevel,
                maxHp = maxHp,
                maxShield = maxShield,
                startingShield = startingShield,
                toughness = toughness,
                attack = attack,
                magic = magic,
                leadership = leadership,
                armor = armor,
                resistance = resistance,
                nonStatCardValueFactor = nonStatCardValueFactor,
                startingResourceAmount = startingResourceAmount,
                maxResourceAmount = maxResourceAmount,
                resourceGainPerTurn = resourceGainPerTurn,
                cardsPerTurn = cardsPerTurn,
                startOfBattleEffects = startOfBattleEffects,
                Cards = deck.Cards
            }
        };
    }
}
