public class StartAdventureV5Requested
{
    public Adventure Adventure { get; }
    public Maybe<BaseHero[]> OverrideHeroes { get; }
    public Maybe<Difficulty> Difficulty;

    public StartAdventureV5Requested(Adventure adventure, Maybe<BaseHero[]> overrideHeroes, Maybe<Difficulty> difficulty)
    {
        Adventure = adventure;
        OverrideHeroes = overrideHeroes;
        Difficulty = difficulty;
    }
}
