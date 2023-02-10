using System.Linq;
using UnityEngine;

public class ChooseCardToPlay : Effect
{
    private readonly int[] _choiceCardIds;
    private readonly string _choicesFormula;

    public ChooseCardToPlay(string choiceCardIds, string choicesFormula)
    {
        _choiceCardIds = string.IsNullOrWhiteSpace(choiceCardIds) 
            ? new int[0] 
            : choiceCardIds.Split(',').Select(int.Parse).ToArray();
        _choicesFormula = choicesFormula;
    }
    
    public void Apply(EffectContext ctx)
    {
        if (_choiceCardIds.Length == 1)
        {
            var owner = ctx.Target.Members[0];
            ctx.PlayerCardZones.HandZone.PutOnBottom(new Card(ctx.GetNextCardId(), owner, ctx.AllCards[_choiceCardIds[0]], 
                ctx.OwnerTints.ContainsKey(owner.Id) ? ctx.OwnerTints[owner.Id] : Maybe<Color>.Missing(), 
                ctx.OwnerBusts.ContainsKey(owner.Id) ? ctx.OwnerBusts[owner.Id] : Maybe<Sprite>.Missing()));
            return;
        }

        var cardsToChoose = Formula.EvaluateToInt(ctx.SourceSnapshot.State, ctx.Source.State, _choicesFormula, ctx.XPaidAmount, ctx.ScopedData);
        
        ctx.Selections.CardSelectionOptions = cardsToChoose >= _choiceCardIds.Length 
            ? _choiceCardIds.Select(x => new Card(ctx.GetNextCardId(), ctx.Source, ctx.AllCards[x], 
                ctx.OwnerTints.ContainsKey(ctx.Source.Id) ? ctx.OwnerTints[ctx.Source.Id] : Maybe<Color>.Missing(), 
                ctx.OwnerBusts.ContainsKey(ctx.Source.Id) ? ctx.OwnerBusts[ctx.Source.Id] : Maybe<Sprite>.Missing())).ToArray()
            : _choiceCardIds
                .Shuffled()
                .Take(cardsToChoose)
                .Select(x => new Card(ctx.GetNextCardId(), ctx.Source, ctx.AllCards[x],
                    ctx.OwnerTints.ContainsKey(ctx.Source.Id) ? ctx.OwnerTints[ctx.Source.Id] : Maybe<Color>.Missing(), 
                    ctx.OwnerBusts.ContainsKey(ctx.Source.Id) ? ctx.OwnerBusts[ctx.Source.Id] : Maybe<Sprite>.Missing()))
                .ToArray();
        ctx.Selections.OnCardSelected = card => Message.Publish(new BonusCardPlayRequested(ctx.Source, new BonusCardDetails(card.BaseType, ResourceQuantity.None)));
    }
}