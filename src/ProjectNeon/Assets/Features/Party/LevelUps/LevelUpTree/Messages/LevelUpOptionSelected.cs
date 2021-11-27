
public class LevelUpOptionSelected
{
    public StaticHeroLevelUpOption Selected { get; }
    public StaticHeroLevelUpOption[] Options { get; }

    public LevelUpOptionSelected(StaticHeroLevelUpOption selected, StaticHeroLevelUpOption[] options)
    {
        Selected = selected;
        Options = options;
    }
}
