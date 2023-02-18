using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoCardCanBeDrawnOfArchetype")]
public class NoCardCanBeDrawnOfArchetype : StaticCardCondition
{
    [SerializeField] private CardPlayZones zones;
    [SerializeField] private StringVariable archetype;
    
    public override bool ConditionMet(CardConditionContext ctx)
        => zones.DrawZone.Cards.None(x => x.Archetypes.Contains(archetype.Value)) 
           && zones.DiscardZone.Cards.None(x => x.Archetypes.Contains(archetype.Value));
    
    public override string Description => $"No card can be drawn of type {archetype.Value}, all of them are in your hand";
}