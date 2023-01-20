public interface EnemyType
{
    public int EnemyId { get; }
    EnemyTier Tier { get; }
    int PowerLevel { get; }
}

public class InMemoryEnemyType : EnemyType
{
    public int EnemyId => 0;
    public EnemyTier Tier { get; set; }
    public int PowerLevel { get; set; }
}