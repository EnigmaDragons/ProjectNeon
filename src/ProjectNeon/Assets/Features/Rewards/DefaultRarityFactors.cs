using System.Collections.Generic;

public class DefaultRarityFactors : RarityFactors
{
    public static readonly Dictionary<Rarity, int> Value = new Dictionary<Rarity, int>
    {
        { Rarity.Starter, 8 },
        { Rarity.Common, 80 },
        { Rarity.Uncommon, 24 },
        { Rarity.Rare, 6 },
        { Rarity.Epic, 2 }
    };

    public int this[Rarity r] => Value[r];
}
