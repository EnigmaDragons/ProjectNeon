public class BuyoutEnemyById : Effect
{
    private readonly int _enemyId;

    public BuyoutEnemyById(string effectContext)
    {
        _enemyId = int.Parse(effectContext);
    }
    
    public void Apply(EffectContext ctx)
    {
        if (ctx.BattleMembers.ContainsKey(_enemyId) && ctx.BattleMembers[_enemyId].IsConscious())
            Message.Publish(new DespawnEnemy(ctx.BattleMembers[_enemyId].State));
    }
}