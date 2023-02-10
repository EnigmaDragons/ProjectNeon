using System.Collections.Generic;
using UnityEngine;

public class InMemoryCard : CardTypeData
{
    public int Id { get; set; } = -1;
    public string Name { get; set; } = "Unnamed";
    public string NameTerm => $"CardNames/{NameKey}";
    public string NameKey => $"{Id.ToString().PadLeft(5, '0')}-00-Name";
    public string DescriptionTerm => $"CardDescriptions/{DescriptionKey}";
    public string DescriptionKey => $"{Id.ToString().PadLeft(5, '0')}-05-Desc";
    public Rarity Rarity { get; set; } = Rarity.Common;
    public IResourceAmount Cost { get; set; } = new InMemoryResourceAmount(0, "None", false);
    public CardSpeed Speed { get; set; } = CardSpeed.Standard;
    public CardActionSequence[] ActionSequences { get; set; } = new CardActionSequence[0];
    public Maybe<CardTypeData> ChainedCard { get; } = Maybe<CardTypeData>.Missing();
    public Maybe<CardTypeData> SwappedCard { get; } = Maybe<CardTypeData>.Missing();
    public HashSet<string> Archetypes { get; set; } = new HashSet<string>();
    public Maybe<CardCondition> HighlightCondition { get; } = Maybe<CardCondition>.Missing();
    public Maybe<CardCondition> UnhighlightCondition { get; } = Maybe<CardCondition>.Missing();
    public Maybe<TargetedCardCondition> TargetedHighlightCondition { get; } = Maybe<TargetedCardCondition>.Missing();
    public Maybe<TargetedCardCondition> TargetedUnhighlightCondition { get; } = Maybe<TargetedCardCondition>.Missing();
    public bool IsSinglePlay { get; set; } = false;

    public Sprite Art { get; set; }
    public string Description { get; set; }
    public CardDescriptionV2 DescriptionV2 { get; set; } = new CardDescriptionV2();
    public HashSet<CardTag> Tags { get; set; }
    public string TypeDescription { get; set; }

    public override string ToString() => Name;
}
