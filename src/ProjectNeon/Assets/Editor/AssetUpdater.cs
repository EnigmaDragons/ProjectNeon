using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class AssetUpdater
{
    [MenuItem("Neon/Update All Assets")]
    public static void Go()
    {
        UpdateCardPools();
        UpdateEquipmentPools();
        UpdateCardIDs();
        UpdateAllCards();
        EnsureDurationPresent();
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
        var cards = ScriptableExtensions.GetAllInstances<CardType>();
        var map = Enumerable.Range(0, cards.Length + 1).ToDictionary(x => x, x => new List<CardType>());
        cards.ForEach(x => map[x.Id].Add(x));
        for (int i = 1; i < map.Count; i++)
        {
            while (map[i].Count > 1)
            {
                var card = map[i][0];
                card.Id = 0;
                map[i].Remove(card);
            }
        }
        for (int i = 1; i < map.Count; i++)
        {
            if (map[i].Count == 0)
            {
                var card = map[0][0];
                card.Id = i;
                EditorUtility.SetDirty(card);
                map[0].Remove(card);
            }
        }
    }

    [MenuItem("Neon/UpdateAllCards")]
    private static void UpdateAllCards()
    {
        var cards = ScriptableExtensions.GetAllInstances<CardType>();
        var allCards = ScriptableExtensions.GetAllInstances<AllCards>();
        allCards.ForEach(x =>
        {
            x.Cards = cards;
            EditorUtility.SetDirty(x);
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
