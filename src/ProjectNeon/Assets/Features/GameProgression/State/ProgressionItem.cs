
public class ProgressionItem
{
    public bool Completed { get; }
    public string Description { get; }

    public string FullDescription()
    {
        var completedWord = Completed ? "Complete" : "Incomplete";
        return $"{completedWord} - {Description}";
    }

    public ProgressionItem(bool completed, string description)
    {
        Completed = completed;
        Description = description;
    }
}
