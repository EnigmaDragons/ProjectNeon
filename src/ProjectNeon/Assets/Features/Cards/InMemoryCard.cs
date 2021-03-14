using System.Collections.Generic;
using UnityEngine;

public class InMemoryCard : CardTypeData
{
    public string Name { get; set; } = "Unnamed";
    public Rarity Rarity { get; set; } = Rarity.Common;
    public IResourceAmount Cost { get; set; } = new InMemoryResourceAmount(0, "None", false);
    public IResourceAmount Gain { get; set; } = new InMemoryResourceAmount(0, "None", false);
    public Maybe<CharacterClass> LimitedToClass { get; } = Maybe<CharacterClass>.Missing();
    public CardActionSequence[] ActionSequences { get; set; } = new CardActionSequence[0];
    public Maybe<CardTypeData> ChainedCard { get; } = Maybe<CardTypeData>.Missing();
    
    public Sprite Art { get; }
    public string Description { get; }
    public HashSet<CardTag> Tags { get; }
    public string TypeDescription { get; }
}
