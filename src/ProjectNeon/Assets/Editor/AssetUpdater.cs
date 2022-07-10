using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetUpdater
{
    [MenuItem("Neon/Update All Assets")]
    public static void Go()
    {
        Timed("Heroes", UpdateHeroes);
        Timed("Adventures", UpdateAdventures);
        Timed("Card Pools", UpdateCardPools);
        Timed("Equipment Pools", UpdateEquipmentPools);
        UpdateLevelUpOptions();
        UpdateCardIDs();
        UpdateAllCards();
        UpdateEquipmentIDs();
        UpdateAllEquipments();
        UpdateAllResourceTypes();
        EnsureDurationPresent();
        UpdateMapIDs();
        UpdateAllMaps();
        UpdateAllHeroes();
        UpdateEnemyIDs();
        UpdateAllEnemies();
        UpdateStageSegmentIDs();
        UpdateAllStageSegments();
        UpdateAllCorps();
        UpdateGlobalEffectIds();
        UpdateAllGlobalEffectsPool();
        UpdateAllTutorialSlideshows();
        Timed("All Battle VFX", UpdateAllBattleVfx);
        Log.Info("Asset Updates Complete");
    }

    private static void Timed(string name, Action a)
    {
        var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        a();
        var stop = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Log.Info($"Updated {name} in {stop - start}ms");
    }

    [MenuItem("Neon/Update/Update All Battle VFX")]
    private static void UpdateAllBattleVfx()
    {
        var guids = AssetDatabase.FindAssets("t:Prefab VFX");

        var battleVfx = new List<BattleVFX>();
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var b = (BattleVFX)go.GetComponentInChildren(typeof(BattleVFX));
            if (b != null)
                battleVfx.Add(b);
        }
        
        ScriptableExtensions.GetAllInstances<AllBattleVfx>().ForEach(x =>
        {
            x.allFx = battleVfx.ToArray();
            EditorUtility.SetDirty(x);
        });
    }

    [MenuItem("Neon/Update/Update Card Pools")]
    private static void UpdateCardPools()
    {
        var cards = ScriptableExtensions.GetAllInstances<CardType>();
        var cardPools = ScriptableExtensions.GetAllInstances<ShopCardPool>();
        foreach (var pool in cardPools)
        {
            var validCards =  cards.Where(card => card.IncludeInPools
                                                  && pool.includedRarities.Contains(card.Rarity)
                                                  && pool.archetypes.Length == card.Archetypes.Count
                                                  && card.Archetypes.All(cardArchetype => pool.archetypes.Any(poolArchetype => poolArchetype.Value == cardArchetype)))
                .ToList();
            if (validCards.Count != pool.allCards.Count 
                || validCards.Any(validCard => !pool.allCards.Contains(validCard)))
            {
                pool.allCards = validCards;
                EditorUtility.SetDirty(pool);
            }
        }
    }

    [MenuItem("Neon/Update/Update Level Up Options")]
    private static void UpdateLevelUpOptions()
    {
        var levelUpOptions = ScriptableExtensions.GetAllInstances<StaticHeroLevelUpOption>();
        AssignAllIds(levelUpOptions, x => x.id, (x, id) => x.id = id);
        ScriptableExtensions.GetAllInstances<AllLevelUpOptions>().ForEach(x =>
        {
            x.LevelUps = levelUpOptions;
            EditorUtility.SetDirty(x);
        });
    }

    [MenuItem("Neon/Update/Update Adventures")]
    private static void UpdateAdventures()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<Adventure>(), x => x.id, (x, id) => x.id = id);
    }
    
    [MenuItem("Neon/Update/Update Heroes")]
    private static void UpdateHeroes()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<BaseHero>(), x => x.id, (x, id) => x.id = id);
    }

    [MenuItem("Neon/Update/Update Static Equipment")]
    private static void UpdateEquipmentIDs()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<StaticEquipment>(), x => x.id, (x, id) => x.id = id);
    }

    private static void AssignAllIds<T>(T[] items, Func<T, int> getId, Action<T, int> setId) where T : Object
    {
        var map = Enumerable.Range(0, items.Length + 1).ToDictionary(x => x, x => new List<T>());
        foreach (var h in items)
        {
            if (map.TryGetValue(getId(h), out var collection))
                collection.Add(h);
            else
                map[0].Add(h);
        }
        for (var i = 1; i < map.Count; i++)
        {
            while (map[i].Count > 1)
            {
                var item = map[i][0];
                setId(item, 0);
                map[i].Remove(item);
            }
        }
        for (var i = 1; i < map.Count; i++)
        {
            if (map[i].Count == 0)
            {
                var item = map[0][0];
                setId(item, i);
                EditorUtility.SetDirty(item);
                map[0].Remove(item);
            }
        }   
    }

    [MenuItem("Neon/Update/Update Equipment Pools")]
    private static void UpdateEquipmentPools()
    {
        var corps = ScriptableExtensions.GetAllInstances<AllCorps>().First();
        var equipments = ScriptableExtensions.GetAllInstances<StaticEquipment>();
        var equipmentPools = ScriptableExtensions.GetAllInstances<EquipmentPool>();
        foreach (var pool in equipmentPools)
        {
            var validEquipments = equipments.Where(equipment => equipment.IncludeInPools
                    && pool.includedRarities.Contains(equipment.Rarity)
                    && pool.archetypes.Length == equipment.Archetypes.Length
                    && equipment.Archetypes.All(equipmentArchetype => pool.archetypes.Any(poolArchetype => poolArchetype.Value == equipmentArchetype)))
                .ToList();
            if (validEquipments.Count != pool.all.Count
                || validEquipments.Any(validEquipment => !pool.all.Contains(validEquipment)))
            {
                pool.all = validEquipments;
                EditorUtility.SetDirty(pool);
            }
            if (pool.corps == null)
            {
                pool.corps = corps;
                EditorUtility.SetDirty(pool);
            }
        }
    }

    [MenuItem("Neon/Update/Update Card IDs")]
    private static void UpdateCardIDs()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<CardType>(), c => c.id, (c, id) => c.id = id);
    }

    [MenuItem("Neon/Update/UpdateAllCards")]
    private static void UpdateAllCards()
    {
        var cards = ScriptableExtensions.GetAllInstances<CardType>().Where(x => x != null).ToArray();
        var allCards = ScriptableExtensions.GetAllInstances<AllCards>();
        allCards.ForEach(x =>
        {
            if (x.Cards.Length != cards.Length || cards.Any(card => !x.Cards.Contains(card)))
            {
                x.Cards = cards;
                EditorUtility.SetDirty(x);   
            }
        });
    }
    
    [MenuItem("Neon/Update/UpdateAllHeroes")]
    private static void UpdateAllHeroes()
    {
        var items = ScriptableExtensions.GetAllInstances<BaseHero>().Where(x => x != null).ToArray();
        var all = ScriptableExtensions.GetAllInstances<AllHeroes>();
        all.ForEach(x =>
        {
            if (x.Heroes.Length != items.Length || items.Any(card => !x.Heroes.Contains(card)))
            {
                x.Heroes = items;
                EditorUtility.SetDirty(x);   
            }
        });
    }


    [MenuItem("Neon/Update/UpdateAllCorps")]
    private static void UpdateAllCorps()
    {
        var corps = ScriptableExtensions.GetAllInstances<StaticCorp>();
        var allCorps = ScriptableExtensions.GetAllInstances<AllCorps>();
        allCorps.ForEach(x =>
        {
            if (x.Corps == null || x.Corps.Length != corps.Length || corps.Any(card => !x.Corps.Contains(card)))
            {
                x.Corps = corps;
                EditorUtility.SetDirty(x);   
            }
        });
    }

    [MenuItem("Neon/Update/UpdateAllEquipments")]
    private static void UpdateAllEquipments()
    {
        var equipments = ScriptableExtensions.GetAllInstances<StaticEquipment>().Where(e => e.IncludeInPools).ToArray();
        var allEquipments = ScriptableExtensions.GetAllInstances<AllEquipment>();
        allEquipments.ForEach(x =>
        {
            x.Equipments = equipments;
            EditorUtility.SetDirty(x);
        });
    }
    
    [MenuItem("Neon/Update/UpdateAllResourceTypes")]
    private static void UpdateAllResourceTypes()
    {
        var resourceTypes = ScriptableExtensions.GetAllInstances<SimpleResourceType>().ToArray();
        var all = ScriptableExtensions.GetAllInstances<AllResourceTypes>();
        all.ForEach(x =>
        {
            x.ResourceTypes = resourceTypes;
            EditorUtility.SetDirty(x);
        });
    }
    
    [MenuItem("Neon/Update/Update Map IDs")]
    private static void UpdateMapIDs()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<GameMap3>(), c => c.id, (c, id) => c.id = id);
    }
    
    [MenuItem("Neon/Update/Update Global Effect IDs")]
    private static void UpdateGlobalEffectIds()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<StaticGlobalEffect>(), c => c.id, (c, id) => c.id = id);
    }

    [MenuItem("Neon/Update/Update Global Effect Pool")]
    private static void UpdateAllGlobalEffectsPool()
    {
        var effects = ScriptableExtensions.GetAllInstances<StaticGlobalEffect>();
        var all = ScriptableExtensions.GetAllInstances<AllStaticGlobalEffects>();
        all.ForEach(x =>
        {
            x.Effects = effects;
            EditorUtility.SetDirty(x);
        });
    }

    [MenuItem("Neon/Update/Update Tutorial Slideshows")]
    private static void UpdateAllTutorialSlideshows()
    {
        var slideshows = ScriptableExtensions.GetAllInstances<TutorialSlideshow>();
        var all = ScriptableExtensions.GetAllInstances<AllTutorialSlideshows>();
        all.ForEach(x =>
        {
            x.tutorials = slideshows;
            EditorUtility.SetDirty(x);
        });
    }

    [MenuItem("Neon/Update/UpdateAllMaps")]
    private static void UpdateAllMaps()
    {
        var maps = ScriptableExtensions.GetAllInstances<GameMap3>();
        var allMaps = ScriptableExtensions.GetAllInstances<AllMaps>();
        allMaps.ForEach(x =>
        {
            x.Maps = maps;
            EditorUtility.SetDirty(x);
        });
    }
    
    [MenuItem("Neon/Update/Update Enemy IDs")]
    private static void UpdateEnemyIDs()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<Enemy>(), c => c.id, (c, id) => c.id = id);
    }
    
    [MenuItem("Neon/Update/UpdateAllEnemies")]
    private static void UpdateAllEnemies()
    {
        var enemies = ScriptableExtensions.GetAllInstances<Enemy>();
        var allEnemies = ScriptableExtensions.GetAllInstances<AllEnemies>();
        allEnemies.ForEach(x =>
        {
            x.Enemies = enemies;
            EditorUtility.SetDirty(x);
        });
    }
    
    [MenuItem("Neon/Update/Update Stage Segment IDs")]
    private static void UpdateStageSegmentIDs()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<StageSegment>(), c => c.Id, (c, id) => c.Id = id);
    }
    
    [MenuItem("Neon/Update/Update All Stage Segments")]
    private static void UpdateAllStageSegments()
    {
        var stageSegments = ScriptableExtensions.GetAllInstances<StageSegment>();
        var allStageSegments = ScriptableExtensions.GetAllInstances<AllStageSegments>();
        allStageSegments.ForEach(x =>
        {
            x.Stages = stageSegments;
            EditorUtility.SetDirty(x);
        });
    }

    private const decimal _hpValue = 1;
    private const decimal _startingShieldValue = 1;
    private const decimal _maxShieldValue = 0.2m;
    private const decimal _armorValue = 3;
    private const decimal _resistanceValue = 3;
    private const decimal _resourceScaledValue = 0.33m;
    private const decimal _maxResourceFactor = 0.1m;
    private const decimal _dodgeScaledValue = 0.5m;
    private const decimal _aegisScaledValue = 0.5m;
    private const decimal _tauntScaledValue = 1m;

    [MenuItem("Neon/Calculate Enemy Power Levels")]
    private static void CalculateEnemyPowerLevels()
    {
        var enemies = ScriptableExtensions.GetAllInstances<Enemy>();
        foreach (var enemy in enemies)
        {
            foreach (var stage in enemy.stageDetails)
            {
                if (stage.stage == 0)
                    return;
                CalculateEnemyPowerLevel(stage, enemy);
            }
        }
    }

    private static void CalculateEnemyPowerLevel(EnemyStageDetails stage, Enemy enemy)
    {
        var previousPlayerDamage = 0m;
        var playerDamage = 10m;
        while ((int)Math.Round(playerDamage) != (int)Math.Round(previousPlayerDamage))
        {
            if (previousPlayerDamage == 0m)
                previousPlayerDamage = 10m;
            previousPlayerDamage = previousPlayerDamage + (playerDamage - previousPlayerDamage) / 2m;
            playerDamage = 0m;
            
            decimal hpValue = _hpValue * stage.maxHp;
            decimal shieldValue = _startingShieldValue * stage.startingShield +
                                  WithFallOff(_maxShieldValue, stage.maxShield, 0.9m);
            decimal aegisValue = WithFallOff(_aegisScaledValue * previousPlayerDamage, stage.startingAegis, 0.8m);
            decimal dodgeValue = _dodgeScaledValue * previousPlayerDamage * stage.startingDodge;
            decimal tauntValue = WithFallOff(_tauntScaledValue * previousPlayerDamage, stage.startingTaunt, 0.8m);
            decimal armorValue = WithFallOff(_armorValue, stage.armor, 0.9m);
            decimal resistanceValue = WithFallOff(_resistanceValue, stage.resistance, 0.9m);
            decimal startingValue = hpValue + shieldValue + aegisValue + dodgeValue + tauntValue + armorValue +
                                    resistanceValue + (decimal)stage.calculationVariables.startingValueAdjustment +
                                    (decimal)stage.calculationVariables.startingDefensiveValueAdjustment;

            decimal highestStat = Mathf.Max(stage.attack, stage.leadership, stage.magic);
            decimal scalingValuePerCard = stage.nonStatCardValueFactor == 0
                ? highestStat
                : (highestStat + (decimal)stage.nonStatCardValueFactor) / 2m;
            decimal resourceValue = stage.calculationVariables.resourceScaledValueOverride == 0
                ? scalingValuePerCard * _resourceScaledValue
                : scalingValuePerCard * (decimal)stage.calculationVariables.resourceScaledValueOverride;
            decimal valuePerTurn = scalingValuePerCard * stage.cardsPerTurn +
                                   resourceValue * stage.resourceGainPerTurn +
                                   (decimal)stage.calculationVariables.perTurnValueAdjustment;
            decimal defensiveValue = hpValue + shieldValue + aegisValue + dodgeValue + armorValue +
                                     resistanceValue + (decimal)stage.calculationVariables.startingDefensiveValueAdjustment;
            decimal averageTurnsAlive = defensiveValue / (previousPlayerDamage * 1.5m);
            decimal activeValue = WithFallOff(valuePerTurn, enemy.isHasty ? averageTurnsAlive : averageTurnsAlive - 1, 0.9m);
            decimal startingResourceValue = resourceValue * stage.startingResourceAmount;
            decimal resourceMaxValue = WithFallOff(_maxResourceFactor * resourceValue, stage.maxResourceAmount, 0.8m);
            decimal totalValue = activeValue + startingValue + resourceMaxValue + startingResourceValue;
            decimal valueCost = 0m;
            while (valueCost + (playerDamage + 1m) * 2m < totalValue)
            {
                playerDamage += 1m;
                valueCost += playerDamage * 2m;
            }
            playerDamage += (totalValue - valueCost) / ((playerDamage + 1) * 2m) ;
        }
        if (Math.Round(playerDamage) < 10)
            FinalEnemyCalculation(stage, enemy, 10);
        else
            FinalEnemyCalculation(stage, enemy, Math.Round(playerDamage));
    }

    private static void FinalEnemyCalculation(EnemyStageDetails stage, Enemy enemy, decimal avgDefensiveValueRemovedPerTurn)
    {
        decimal hpValue = _hpValue * stage.maxHp;
        decimal shieldValue = _startingShieldValue * stage.startingShield +
                              WithFallOff(_maxShieldValue, stage.maxShield, 0.9m);
        decimal aegisValue = WithFallOff(_aegisScaledValue * avgDefensiveValueRemovedPerTurn, stage.startingAegis, 0.8m);
        decimal dodgeValue = _dodgeScaledValue * avgDefensiveValueRemovedPerTurn * stage.startingDodge;
        decimal tauntValue = WithFallOff(_tauntScaledValue * avgDefensiveValueRemovedPerTurn, stage.startingTaunt, 0.8m);
        decimal armorValue = WithFallOff(_armorValue, stage.armor, 0.9m);
        decimal resistanceValue = WithFallOff(_resistanceValue, stage.resistance, 0.9m);
        decimal startingValue = hpValue + shieldValue + aegisValue + dodgeValue + tauntValue + armorValue +
                                resistanceValue + (decimal)stage.calculationVariables.startingValueAdjustment +
                                (decimal)stage.calculationVariables.startingDefensiveValueAdjustment;

        decimal highestStat = Mathf.Max(stage.attack, stage.leadership, stage.magic);
        decimal scalingValuePerCard = stage.nonStatCardValueFactor == 0
            ? highestStat
            : (highestStat + (decimal)stage.nonStatCardValueFactor) / 2m;
        decimal resourceValue = stage.calculationVariables.resourceScaledValueOverride == 0
            ? scalingValuePerCard * _resourceScaledValue
            : scalingValuePerCard * (decimal)stage.calculationVariables.resourceScaledValueOverride;
        decimal valuePerTurn = scalingValuePerCard * stage.cardsPerTurn +
                               resourceValue * stage.resourceGainPerTurn +
                               (decimal)stage.calculationVariables.perTurnValueAdjustment;
        decimal defensiveValue = hpValue + shieldValue + aegisValue + dodgeValue + armorValue +
                                 resistanceValue + (decimal)stage.calculationVariables.startingDefensiveValueAdjustment;
        decimal averageTurnsAlive = defensiveValue / (avgDefensiveValueRemovedPerTurn * 1.5m);
        decimal activeValue = WithFallOff(valuePerTurn, averageTurnsAlive, 0.9m);
        decimal startingResourceValue = resourceValue * stage.startingResourceAmount;
        decimal resourceMaxValue = WithFallOff(_maxResourceFactor * resourceValue, stage.maxResourceAmount, 0.8m);
        int totalValue = (int)Math.Round((double)(activeValue + startingValue + resourceMaxValue + startingResourceValue));
        if (totalValue != stage.calculationResults.calculatedPowerLevel)
        {
            stage.calculationResults.startingPower = (float)startingValue;
            stage.calculationResults.perCardValue = (float)scalingValuePerCard;
            stage.calculationResults.resourceValue = (float)resourceValue;
            stage.calculationResults.perTurnPower = (float)valuePerTurn;
            stage.calculationResults.maxAndStartingResourcesPower = (float)(startingResourceValue + resourceMaxValue);
            stage.calculationResults.estimatedTurnsAlive = (float)averageTurnsAlive;
            stage.calculationResults.calculatedPowerLevel = totalValue;
            stage.calculationResults.estimatedHeroDamage = (float)avgDefensiveValueRemovedPerTurn;
            stage.calculationResults.estimatedPerTurnTotalValue = (float)activeValue;
            EditorUtility.SetDirty(enemy);
        }
    }

    private static decimal WithFallOff(decimal value, decimal amount, decimal fallOff)
    {
        if (amount == 0)
            return 0;
        var currentValue = value;
        var i = 2;
        for (; i <= amount; i++)
            currentValue += value * (decimal)Math.Pow((double)fallOff, i - 1);
        currentValue += value * (decimal)Math.Pow((double)fallOff, (double)amount) * (amount - i + 1);
        return currentValue;
    }
    
    private static void EnsureDurationPresent()
    {
        var cardActionsDatas = ScriptableExtensions.GetAllInstances<CardActionsData>();
        foreach (var cardActionsData in cardActionsDatas)
        {
            var isDirty = false;
            foreach (var battleEffect in cardActionsData.BattleEffects)
            {
                if (string.IsNullOrWhiteSpace(battleEffect.DurationFormula))
                {
                    isDirty = true;
                    battleEffect.DurationFormula = "0";
                }
            }
            if (isDirty)
                EditorUtility.SetDirty(cardActionsData);
        }
    }
}
