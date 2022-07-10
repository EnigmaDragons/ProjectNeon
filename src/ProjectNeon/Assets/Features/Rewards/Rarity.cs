using System;
using System.Collections.Generic;
using System.Linq;

public enum Rarity
{
    Starter = 0, // Simple cards, 0.8x power level
    Basic = 5, // Character Specific Card 1.0x power level
    Common = 1, // Normal power level
    Uncommon = 2, // 1.2x power level, slightly more complex
    Rare = 3, // 1.5x power level, some unique mechanics and more complex cards
    Epic = 4 // 2x power level, should do wild things
}

public static class RarityExtensions
{
    private static IEnumerable<Rarity> Factored(RarityFactors f, Rarity r) => Enumerable.Range(0, f[r]).Select(_ => r);

    public static IEnumerable<T> FactoredByRarity<T>(this IEnumerable<T> items, RarityFactors factors, Func<T, Rarity> getRarity)
        => items.SelectMany(item => Enumerable.Range(0, factors[getRarity(item)]).Select(_ => item));

    public static IEnumerable<Rarity> Random(this Rarity[] rarities, RarityFactors f, int n)
        => Enumerable.Range(0, n).Select(_ => f.Random(rarities));

    public static Rarity RandomExceptStarters(this RarityFactors f) => Random(f, AllExceptStarters);
    public static Rarity Random(this RarityFactors f, params Rarity[] rarities)
        => rarities.SelectMany(r => Factored(f, r)).Random();
    
    public static Rarity[] AllExceptStarters { get; } = {Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic};
}

