using System.Linq;

public class ChooseCardToCreate : Effect
{
    private readonly int[] _choiceCardIds;
    private readonly int _choiceCount;

    public ChooseCardToCreate(string choiceCardIds, int choiceCount)
    {
        _choiceCardIds = choiceCardIds.Split(',').Select(int.Parse).ToArray();
        _choiceCount = choiceCount;
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.Selections.CardSelectionOptions = _choiceCardIds.Shuffled().Take(_choiceCount).Select(x => new Card(-1, ctx.Source, ctx.AllCards[x])).ToArray();
        ctx.Selections.OnCardSelected = card => ctx.PlayerCardZones.HandZone.PutOnBottom(card);
    }
}