using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoCardCanBeDrawnOfArchetype")]
public class NoCardCanBeDrawnOfArchetype : StaticCardCondition
{
    [SerializeField] private CardPlayZones zones;
    [SerializeField] private StringVariable archetype;
    
    public override bool ConditionMet(CardConditionContext ctx)
        => zones.DrawZone.Cards.None(x => x.Archetypes.Contains(archetype.Value)) 
           && zones.DiscardZone.Cards.None(x => x.Archetypes.Contains(archetype.Value));
    
    public override string Description => "Thoughts/Condition016".ToLocalized().SafeFormatWithDefault("No card can be drawn of type {0}, all of them are in your hand", $"Archetypes/{archetype.Value}".ToLocalized());
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition016" };
}