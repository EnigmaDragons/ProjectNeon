public class StartAdventureV5Requested
{
    public Adventure Adventure { get; }
    public Maybe<BaseHero[]> OverrideHeroes { get; }

    public StartAdventureV5Requested(Adventure adventure, Maybe<BaseHero[]> overrideHeroes)
    {
        Adventure = adventure;
        OverrideHeroes = overrideHeroes;
    }
}
