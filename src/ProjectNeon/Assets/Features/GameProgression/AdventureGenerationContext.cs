
public class AdventureGenerationContext
{
    public AdventureProgress2 Adventure { get; }
    public AllEnemies Enemies { get; }

    public AdventureGenerationContext(AdventureProgress2 adventure, AllEnemies enemies)
    {
        Adventure = adventure;
        Enemies = enemies;
    }
}
