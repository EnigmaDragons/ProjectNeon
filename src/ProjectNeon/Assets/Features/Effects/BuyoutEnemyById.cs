public class BuyoutEnemyById : Effect
{
    private readonly int _enemyId;

    public BuyoutEnemyById(string effectContext)
    {
        _enemyId = int.TryParse(effectContext, out var val) 
            ? val 
            : -1;
    }
    
    public void Apply(EffectContext ctx)
    {
        if (ctx.BattleMembers.ContainsKey(_enemyId) && ctx.BattleMembers[_enemyId].IsConscious())
            Message.Publish(new DespawnEnemy(ctx.BattleMembers[_enemyId]));
    }
}