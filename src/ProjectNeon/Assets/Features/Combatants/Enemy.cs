using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    [SerializeField] private string enemyName;
    [SerializeField] private string lastBalanceDate = "Never";
    [SerializeField] private bool isCurrentlyWorking = true;
    [SerializeField] private TurnAI ai;
    [SerializeField] private int preferredTurnOrder = 99;
    [SerializeField] private GameObject prefab;
    [SerializeField] private StringReference deathEffect;
    [SerializeField] private BattleRole battleRole;
    [SerializeField] private EnemyTier tier; 
    [SerializeField] private bool unique;
    [SerializeField] private bool isHasty;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private EnemyStageDetails[] stageDetails = new EnemyStageDetails[0];

    public EnemyInstance GetEnemy(int stage)
    {
        var detail = stageDetails.OrderBy(x => x.stage > stage ? Math.Abs(x.stage - stage) * 2 + 1 : Math.Abs(x.stage - stage) * 2).FirstOrDefault();
        if (detail == null)
            Log.Error($"Enemy {enemyName} has no stage details and can not be used");
        return new EnemyInstance(id, resourceType, detail.startOfBattleEffects, detail.startingResourceAmount, detail.resourceGainPerTurn, 
            detail.maxResourceAmount, detail.maxHp, detail.maxShield, detail.startingShield, detail.toughness, 
            detail.attack, detail.magic, detail.leadership, detail.armor, detail.resistance, detail.cardsPerTurn, 
            prefab, ai, detail.Cards, battleRole, tier, detail.powerLevel, preferredTurnOrder, enemyName, deathEffect, 
            isHasty, unique, detail.CounterAdjustments);
    } 
    public EffectData[] Effects => stageDetails.SelectMany(x => x.startOfBattleEffects).ToArray();
}
