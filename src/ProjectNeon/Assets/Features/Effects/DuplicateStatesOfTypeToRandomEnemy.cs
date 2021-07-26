using System.Linq;

public class DuplicateStatesOfTypeToRandomEnemy : Effect
{
    private readonly StatusTag _tag;

    public DuplicateStatesOfTypeToRandomEnemy(StatusTag tag)
    {
        _tag = tag;
    }

    public void Apply(EffectContext ctx)
    {
        if (_tag == StatusTag.None)
            return;
        var toTransferTo = ctx.BattleMembers.Where(x => x.Value.IsConscious() && x.Key != ctx.Target.Members[0].Id && x.Value.TeamType == ctx.Target.Members[0].TeamType)
            .Select(x => x.Value)
            .ToArray()
            .Shuffled()
            .FirstOrDefault();
        toTransferTo?.State.DuplicateStatesOfTypeFrom(_tag, ctx.Target.Members[0].State);
    }
}