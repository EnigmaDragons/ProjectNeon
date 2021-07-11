using System.Collections.Generic;
using System.Linq;

public class ChooseAndDrawCardOfArchetype : Effect
{
    private readonly string archetype;

    public ChooseAndDrawCardOfArchetype(string archetype)
    {
        this.archetype = archetype;
    }
    
    public void Apply(EffectContext ctx)
    {
        var possibleCards = new List<Card>();
        possibleCards.AddRange(ctx.PlayerCardZones.DrawZone.Cards.Where(x => x.Archetypes.Contains(archetype)));
        //possibleCards.AddRange(ctx.PlayerCardZones.DiscardZone.Cards.Where(x => x.Archetypes.Contains(archetype)));
        ctx.Selections.CardSelectionOptions = possibleCards.Shuffled().ToArray();
        ctx.Selections.OnCardSelected = card =>
        {
            if (ctx.PlayerCardZones.DiscardZone.Cards.Contains(card))
                ctx.PlayerCardZones.DiscardZone.Remove(card);
            if (ctx.PlayerCardZones.DrawZone.Cards.Contains(card))
                ctx.PlayerCardZones.DrawZone.Remove(card);
            ctx.PlayerCardZones.HandZone.PutOnBottom(card);
        };
    }
}