using System.Linq;
using UnityEngine;

public class ChooseCardToCreate : Effect
{
    private readonly int[] _choiceCardIds;
    private readonly string _choicesFormula;

    public ChooseCardToCreate(string choiceCardIds, string choicesFormula)
    {
        _choiceCardIds = choiceCardIds.Split(',').Select(int.Parse).ToArray();
        _choicesFormula = choicesFormula;
    }
    
    public void Apply(EffectContext ctx)
    {
        var cardsToChoose = Mathf.CeilToInt(Formula.Evaluate(ctx.SourceSnapshot.State, ctx.Source.State, _choicesFormula, ctx.XPaidAmount));
        
        ctx.Selections.CardSelectionOptions = cardsToChoose >= _choiceCardIds.Length 
            ? _choiceCardIds.Select(x => new Card(-1, ctx.Source, ctx.AllCards[x])).ToArray()
            : _choiceCardIds
                .Shuffled()
                .Take(cardsToChoose)
                .Select(x => new Card(-1, ctx.Source, ctx.AllCards[x]))
                .ToArray();
        ctx.Selections.OnCardSelected = card => ctx.PlayerCardZones.HandZone.PutOnBottom(card);
    }
}