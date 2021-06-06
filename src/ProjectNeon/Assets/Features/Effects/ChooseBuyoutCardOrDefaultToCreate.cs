using System.Linq;

public class ChooseBuyoutCardOrDefaultToCreate : Effect
{
    private readonly int[] _otherOptions;

    public ChooseBuyoutCardOrDefaultToCreate(string effectContext)
    {
        _otherOptions = effectContext.Split(',').Select(x => int.Parse(x)).ToArray();
    }
    
    public void Apply(EffectContext ctx)
    {
        /*ctx.Selections.CardSelectionOptions = ctx.BattleMembers.Where(x => x.)
        ctx.Selections.OnCardSelected = card => ctx.PlayerCardZones.HandZone.PutOnBottom(card);*/
    }
}