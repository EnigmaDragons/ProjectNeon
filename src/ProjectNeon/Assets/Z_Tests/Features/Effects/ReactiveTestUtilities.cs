using System.Linq;

public static class ReactiveTestUtilities
{
    public static void ApplyEffectAndReactions(EffectData e, Member source, Member target)
    {
        var battleSnapshotBefore = new BattleStateSnapshot(target.GetSnapshot());
        AllEffects.Apply(e, source, target);
        var battleSnapshotAfter = new BattleStateSnapshot(target.GetSnapshot());

        var effectResolved = new EffectResolved(e, source, new Single(target), battleSnapshotBefore, battleSnapshotAfter);

        var reactions = target.State.GetReactions(effectResolved);
        reactions.ForEach(r => r.Reaction.ActionSequence.CardActions.Actions.Where(a => a.Type == CardBattleActionType.Battle)
            .ForEach(be => AllEffects.Apply(be.BattleEffect, r.Source, r.Target)));
    }
}