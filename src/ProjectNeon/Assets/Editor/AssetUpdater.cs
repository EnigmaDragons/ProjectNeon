using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        UpdateCardIDs();
        UpdateAllCards();
        EnsureDurationPresent();
        Log.Info("Asset Updates Complete");
    }

    [MenuItem("Neon/Update Card Pools")]
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

    
    [MenuItem("Neon/Update Adventures")]
    private static void UpdateAdventures()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<Adventure>(), h => h.id, (h, id) => h.id = id);
    }
    
    [MenuItem("Neon/Update Heroes")]
    private static void UpdateHeroes()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<BaseHero>(), h => h.id, (h, id) => h.id = id);
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

    [MenuItem("Neon/Update Equipment Pools")]
    private static void UpdateEquipmentPools()
    {
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
        }
    }

    [MenuItem("Neon/Update Card IDs")]
    private static void UpdateCardIDs()
    {
        AssignAllIds(ScriptableExtensions.GetAllInstances<CardType>(), c => c.id, (c, id) => c.id = id);
    }

    [MenuItem("Neon/UpdateAllCards")]
    private static void UpdateAllCards()
    {
        var cards = ScriptableExtensions.GetAllInstances<CardType>();
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
