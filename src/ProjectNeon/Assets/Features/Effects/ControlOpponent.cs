using System.Linq;

public class ControlOpponent : Effect
{
    public void Apply(EffectContext ctx)
    {
        //Currently this only works on one opponent 
        var targetMember = ctx.Target.Members[0];
        var enemy = ctx.Enemies[targetMember.Id];
        ctx.Selections.CardSelectionOptions = enemy.Cards.Select(x => x.CreateInstance(-1, targetMember)).ToArray();
        ctx.Selections.OnCardSelected = card =>
        {
            if (card.Type.RequiresPlayerTargeting())
                
            else
                enemy.AI.LockedInDecision = new PlayedCardV2(
                    targetMember, 
                    card.ActionSequences.Select(sequence => ctx.BattleMembers.Values.ToArray().GetPossibleConsciousTargets(targetMember, sequence.Group, sequence.Scope)[0]).ToArray(), 
                    card);
        };
    }
}