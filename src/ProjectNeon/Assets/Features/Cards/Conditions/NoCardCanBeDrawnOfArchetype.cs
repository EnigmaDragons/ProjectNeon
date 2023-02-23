using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoCardCanBeDrawnOfArchetype")]
public class NoCardCanBeDrawnOfArchetype : StaticCardCondition
{
    [SerializeField] private CardPlayZones zones;
    [SerializeField] private StringVariable archetype;
    
    public override bool ConditionMet(CardConditionContext ctx)
        => zones.DrawZone.Cards.None(x => x.Archetypes.Contains(archetype.Value)) 
           && zones.DiscardZone.Cards.None(x => x.Archetypes.Contains(archetype.Value));
    
    public override string Description => string.Format("Thoughts/Condition016".ToLocalized(), $"Archetypes/{archetype.Value}".ToLocalized());
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition016" };
}