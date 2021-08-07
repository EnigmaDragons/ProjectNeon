using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourcePlayedCardOfArchetype")]
public class SourcePlayedCardOfArchetype : StaticEffectCondition
{
    [SerializeField] private StringVariable archetype;
    
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Card.IsPresentAnd(c => c.Archetypes.Contains(archetype.Value))
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} did not play a card of archetype {archetype}.");
    }
}