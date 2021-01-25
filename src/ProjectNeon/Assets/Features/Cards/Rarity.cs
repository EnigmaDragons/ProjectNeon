using System;
using System.Collections.Generic;
using System.Linq;

public enum Rarity
{
    Starter = 0, // Simple cards, normal power level
    Common = 1, // Normal power level
    Uncommon = 2, // 1.2x power level, slightly more complex
    Rare = 3, // 1.5x power level, some unique mechanics and more complex cards
    Epic = 4 // 2x power level, should do wild things
}

public static class RarityExtensions
{
    public static readonly Dictionary<Rarity, int> RarityFactors = new Dictionary<Rarity, int>
    {
        { Rarity.Starter, 8 },
        { Rarity.Common, 80 },
        { Rarity.Uncommon, 24 },
        { Rarity.Rare, 6 },
        { Rarity.Epic, 2 }
    };

    private static IEnumerable<Rarity> Factored(Rarity r) => Enumerable.Range(0, RarityFactors[r]).Select(_ => r);

    public static IEnumerable<T> FactoredByRarity<T>(this IEnumerable<T> items, Func<T, Rarity> getRarity)
        => items.SelectMany(item => Enumerable.Range(0, RarityFactors[getRarity(item)]).Select(_ => item));

    public static IEnumerable<Rarity> Random(this Rarity[] rarities, int n)
        => Enumerable.Range(0, n).Select(_ => rarities.Random());
    public static Rarity Random(this Rarity[] rarities)
        => rarities.SelectMany(Factored).Random();

    public static Rarity RandomNonStarter() => Random(new[] {Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic});
}

