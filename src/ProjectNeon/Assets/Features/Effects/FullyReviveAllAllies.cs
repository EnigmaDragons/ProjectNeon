using System.Linq;

public class FullyReviveAllAllies : Effect
{
    public void Apply(EffectContext ctx)
    {
        var team = ctx.Source.TeamType;
        foreach (var member in ctx.BattleMembers.Values.Where(m => m.TeamType == team && !m.IsConscious()))
            member.Apply(m =>
            {
                m.CleanseDebuffs();
                m.GainHp(9999);
            });
    }
}
