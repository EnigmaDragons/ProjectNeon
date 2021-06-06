public interface EnemyType
{
    string Name { get; }
    EnemyTier Tier { get; }
    int PowerLevel { get; }
}

public class InMemoryEnemyType : EnemyType
{
    public string Name { get; set; }
    public EnemyTier Tier { get; set; }
    public int PowerLevel { get; set; }
}