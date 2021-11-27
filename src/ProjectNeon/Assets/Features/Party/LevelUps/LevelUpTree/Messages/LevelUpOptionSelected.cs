
public class LevelUpOptionSelected
{
    public LevelUpOption Selected { get; }
    public LevelUpOption[] Options { get; }

    public LevelUpOptionSelected(LevelUpOption selected, LevelUpOption[] options)
    {
        Selected = selected;
        Options = options;
    }
}
