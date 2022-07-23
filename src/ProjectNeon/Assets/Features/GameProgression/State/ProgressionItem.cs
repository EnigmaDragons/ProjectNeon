
public class ProgressionItem
{
    public bool Completed { get; }
    public string Description { get; }
    public Maybe<BaseHero> Hero { get; }
    public Maybe<Adventure> Adventure { get; }

    public string FullDescription()
    {
        var completedWord = Completed ? "Complete" : "Incomplete";
        return $"{completedWord} - {Description}";
    }

    public ProgressionItem(bool completed, string description, Maybe<BaseHero> hero, Maybe<Adventure> adventure)
    {
        Completed = completed;
        Description = description;
        Hero = hero;
        Adventure = adventure;
    }
}
