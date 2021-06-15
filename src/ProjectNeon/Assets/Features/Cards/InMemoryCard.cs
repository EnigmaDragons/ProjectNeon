using System.Collections.Generic;
using UnityEngine;

public class InMemoryCard : CardTypeData
{
    public int Id { get; set; } = -1;
    public string Name { get; set; } = "Unnamed";
    public Rarity Rarity { get; set; } = Rarity.Common;
    public IResourceAmount Cost { get; set; } = new InMemoryResourceAmount(0, "None", false);
    public IResourceAmount Gain { get; set; } = new InMemoryResourceAmount(0, "None", false);
    public CardSpeed Speed { get; set; } = CardSpeed.Standard;
    public CardActionSequence[] ActionSequences { get; set; } = new CardActionSequence[0];
    public Maybe<CardTypeData> ChainedCard { get; } = Maybe<CardTypeData>.Missing();
    public Maybe<CardTypeData> SwappedCard { get; } = Maybe<CardTypeData>.Missing();
    public HashSet<string> Archetypes { get; set; } = new HashSet<string>();
    public Maybe<CardCondition> HighlightCondition { get; } = Maybe<CardCondition>.Missing();
    public Maybe<CardCondition> UnhighlightCondition { get; } = Maybe<CardCondition>.Missing();
    public bool IsSinglePlay { get; set; } = false;

    public Sprite Art { get; set; }
    public string Description { get; set; }
    public HashSet<CardTag> Tags { get; set; }
    public string TypeDescription { get; set; }
}
