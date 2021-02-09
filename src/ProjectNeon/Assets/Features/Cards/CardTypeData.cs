using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface CardTypeData 
{
    string Name { get; }
    IResourceAmount Cost { get; }
    IResourceAmount Gain  { get; }
    Sprite Art  { get; }
    string Description  { get; }
    HashSet<CardTag> Tags  { get; }
    string TypeDescription  { get; }
    Maybe<CharacterClass> LimitedToClass  { get; }
    CardActionSequence[] ActionSequences  { get; }
    Maybe<CardTypeData> ChainedCard { get; }
    Rarity Rarity { get; }
    CardTimingType TimingType { get; }
}

public static class CardTypeDataExtensions
{
    public static bool Is(this CardTypeData c, params CardTag[] tags) => tags.All(tag => c.Tags.Contains(tag));

    public static CardActionsData[] Actions(this CardTypeData c) => c.ActionSequences.Select(a => a.CardActions).ToArray();
    
    public static Card CreateInstance(this CardTypeData c, int id, Member owner) => new Card(id, owner, c);
}
