using System.Linq;

public class ResolveInnerEffect : Effect
{
    private CardActionsData _data;
    
    public ResolveInnerEffect(CardActionsData data) => _data = data;

    public void Apply(EffectContext ctx)
    {
        if (_data?.Actions == null)
            return;
        MessageGroup.Add(new MultiplePayloads("inner effect", _data.Actions
            .Where(x => x.Type == CardBattleActionType.Battle || x.Type == CardBattleActionType.SpawnEnemy)
            .Select(action => action.Type == CardBattleActionType.SpawnEnemy 
                ? (object)new SpawnEnemy(action.EnemyToSpawn, action.EnemySpawnOffset, ctx.Source, action.Replacing, ctx.Card, ctx.XPaidAmount, ctx.PaidAmount, ctx.IsReaction, ctx.Timing, new Maybe<EffectCondition>(action.EnemySpawnCondition))
                : (object)new ApplyBattleEffect(false, action.BattleEffect, ctx.Source, ctx.Target, ctx.Card, ctx.XPaidAmount, ctx.PaidAmount, ctx.Preventions, ctx.IsReaction, ctx.Timing))
            .ToArray()));
    }
}