
public class LevelUpOptionSelected
{
    public HeroLevelUpOption Selected { get; }
    public HeroLevelUpOption[] Options { get; }

    public LevelUpOptionSelected(HeroLevelUpOption selected, HeroLevelUpOption[] options)
    {
        Selected = selected;
        Options = options;
    }
}
