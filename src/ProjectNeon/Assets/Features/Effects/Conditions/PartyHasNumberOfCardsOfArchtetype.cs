using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/PartyHasNumberOfCardsOfArchtetype")]
public class PartyHasNumberOfCardsOfArchtetype : StaticEffectCondition
{
    [SerializeField] private StringVariable[] archetypes;
    [SerializeField] private int number;
    [SerializeField] private CardPlayZone zone;
    
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var archetypeKey = string.Join(" + ", archetypes.Select(a => a.Value).OrderBy(a => a));
        return zone.Cards.Count(x => x.GetArchetypeKey() == archetypeKey) == number
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{zone.name} does not have {number} {archetypeKey} cards.");
    }
}
