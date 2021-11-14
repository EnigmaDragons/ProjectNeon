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
        UpdateHeroes();
        UpdateAdventures();
        UpdateCardPools();
        UpdateEquipmentPools();
        UpdateLevelUpOptions();
        UpdateCardIDs();
        UpdateAllCards();
        UpdateEquipmentIDs();
        UpdateAllEquipments();
        EnsureDurationPresent();
        UpdateMapIDs();
        UpdateAllMaps();
        UpdateAllHeroes();
        UpdateEnemyIDs();
        UpdateAllEnemies();
        UpdateAllCorps();
        UpdateStoryEventIDs();
        UpdateGlobalEffectIds();
        UpdateAllGlobalEffectsPool();
        Log.Info("Asset Updates Complete");
    }

    [MenuItem("Neon/Update/Update Card Pools")]
    private static void UpdateCardPools()
    {
        var cards = ScriptableExtensions.GetAllInstances<CardType>();
        var cardPools = ScriptableExtensions.GetAllInstances<ShopCardPool>();
        foreach (var pool in cardPools)
        {
            var validCards =  cards.Where(card => !card.IsWip
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
        var levelUpOptions = ScriptableExtensions.GetAllInstances<HeroLevelUpOption>();
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
            var validEquipments = equipments.Where(equipment => !equipment.IsWip
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
        var equipments = ScriptableExtensions.GetAllInstances<StaticEquipment>();
        var allEquipments = ScriptableExtensions.GetAllInstances<AllEquipment>();
        allEquipments.ForEach(x =>
        {
            x.Equipments = equipments;
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
    
    [MenuItem("Neon/Update/Update Story Event IDs")]
    private static void UpdateStoryEventIDs()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<StoryEvent2>(), c => c.id, (c, id) => c.id = id);
    }

    private const float _hpValue = 1;
    private const float _startingShieldValue = 1;
    private const float _maxShieldValue = 0.2f;
    private const float _dodgeValue = 5;
    private const float _aegisValue = 5;
    private const float _tauntValue = 10;
    private const float _armorValue = 3;
    private const float _resistanceValue = 3;
    private const float _resourceScaledValue = 0.33f;
    private const float _maxResourceFactor = 0.2f;
    private static Dictionary<int, float> _stageDefensiveValuePerTurnAliveMap = new Dictionary<int, float>()
    {
        { 1, 10f },
        { 2, 20f },
        { 3, 40f },
    };

    [MenuItem("Neon/Calculate Enemy Power Levels")]
    private static void CalculateEnemyPowerLevels()
    {
        var enemies = ScriptableExtensions.GetAllInstances<Enemy>();
        foreach (var enemy in enemies)
        {
            foreach (var stage in enemy.stageDetails)
            {
                var hpValue = _hpValue * stage.maxHp;
                var shieldValue = _startingShieldValue * stage.startingShield + WithFallOff(_maxShieldValue, stage.maxShield, 0.9f);
                var aegisValue = WithFallOff(_aegisValue, stage.startingAegis, 0.8f);
                var dodgeValue = _dodgeValue * stage.startingDodge;
                var tauntValue = WithFallOff(_tauntValue, stage.startingTaunt, 0.8f);
                var armorValue = WithFallOff(_armorValue, stage.armor, 0.9f);
                var resistanceValue = WithFallOff(_resistanceValue, stage.resistance, 0.9f);
                var startingValue = hpValue + shieldValue + aegisValue + dodgeValue + tauntValue + armorValue + resistanceValue + stage.calculationVariables.startingValueAdjustment + stage.calculationVariables.startingDefensiveValueAdjustment;

                var highestStat = Mathf.Max(stage.attack, stage.leadership, stage.magic);
                var scalingValuePerCard = stage.nonStatCardValueFactor == 0
                    ? highestStat
                    : (highestStat + stage.nonStatCardValueFactor) / 2f;
                var resourceValue = stage.calculationVariables.resourceScaledValueOverride == 0
                    ? scalingValuePerCard * _resourceScaledValue
                    : scalingValuePerCard * stage.calculationVariables.resourceScaledValueOverride;
                var valuePerTurn = scalingValuePerCard * stage.cardsPerTurn + resourceValue * stage.resourceGainPerTurn + stage.calculationVariables.perTurnValueAdjustment;
                var defensiveValue = hpValue + shieldValue + aegisValue + dodgeValue + armorValue + resistanceValue + stage.calculationVariables.startingDefensiveValueAdjustment;
                var averageTurnsAlive = defensiveValue / _stageDefensiveValuePerTurnAliveMap[stage.stage];
                var activeValue = WithFallOff(valuePerTurn, Mathf.RoundToInt(averageTurnsAlive), 0.9f);
                var startingResourceValue = resourceValue * stage.startingResourceAmount;
                var resourceMaxValue = WithFallOff(_maxResourceFactor * resourceValue, stage.maxResourceAmount, 0.8f);
                int totalValue = Mathf.RoundToInt(activeValue + startingValue + resourceMaxValue + startingResourceValue);
                if (totalValue != stage.calculationResults.calculatedPowerLevel)
                {
                    stage.calculationResults.startingPower = startingValue;
                    stage.calculationResults.perCardValue = scalingValuePerCard;
                    stage.calculationResults.resourceValue = resourceValue;
                    stage.calculationResults.perTurnPower = valuePerTurn;
                    stage.calculationResults.maxAndStartingResourcesPower = startingResourceValue + resourceMaxValue;
                    stage.calculationResults.estimatedTurnsAlive = averageTurnsAlive;
                    stage.calculationResults.calculatedPowerLevel = totalValue;
                    EditorUtility.SetDirty(enemy);
                }
            }
        }
    }

    private static float WithFallOff(float value, int amount, float fallOff)
    {
        if (amount == 0)
            return 0;
        var currentValue = value;
        for (var i = 1; i < amount; i++)
            currentValue += value * Mathf.Pow(fallOff, i);
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
