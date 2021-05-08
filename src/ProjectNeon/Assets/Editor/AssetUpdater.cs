using System.Linq;
using UnityEditor;

public class AssetUpdater
{
    [MenuItem("Neon/Update All Assets")]
    public static void Go()
    {
        UpdateCardPools();
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
            if (validCards.Count != pool.allCards.Count ||
                validCards.Any(validCard => !pool.allCards.Contains(validCard)))
            {
                pool.allCards = validCards;
                EditorUtility.SetDirty(pool);
            }
        }
    }
}
