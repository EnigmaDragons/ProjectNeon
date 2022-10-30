public interface EnemyType
{
    EnemyTier Tier { get; }
    int PowerLevel { get; }
}

public class InMemoryEnemyType : EnemyType
{
    public EnemyTier Tier { get; set; }
    public int PowerLevel { get; set; }
}