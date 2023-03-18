
public class ProgressionItem
{
    public bool Completed { get; }
    public string Description { get; }
    public Maybe<BaseHero> Hero { get; }
    public Maybe<Adventure> Adventure { get; }
    public Maybe<Difficulty> Difficulty { get; }
    public Maybe<Boss> Boss { get; }

    public string FullDescription()
    {
        var completedWord = (Completed ? "Progressions/Complete" : "Progressions/Incomplete").ToLocalized();
        return $"{completedWord} - {Description}";
    }
    
    public ProgressionItem(bool completed, string description, Maybe<BaseHero> hero, Maybe<Adventure> adventure, Maybe<Difficulty> difficulty, Maybe<Boss> boss)
    {
        Completed = completed;
        Description = description;
        Hero = hero;
        Adventure = adventure;
        Difficulty = difficulty;
        Boss = boss;
    }
}
