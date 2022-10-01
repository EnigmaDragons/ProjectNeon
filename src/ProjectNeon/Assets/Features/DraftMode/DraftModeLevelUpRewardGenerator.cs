using System.Linq;

public static class DraftModeLevelUpRewardGenerator
{
    private static (string description, StatAddends stats)[] statOptions =
    {
        ("+8 MaxHP", new StatAddends().With(StatType.MaxHP, 8)),
        ("+1 Armor", new StatAddends().With(StatType.Armor, 1)),
        ("+1 Resistance", new StatAddends().With(StatType.Resistance, 1)),
        ("+1 Power", new StatAddends().With(StatType.Power, 1)),
        ("+16 Max Shield", new StatAddends().With(StatType.MaxShield, 16)),
        ("+4 Starting Shield", new StatAddends().With(StatType.StartingShield, 4)),
    };
    
    public static LevelUpOption[] GenerateOptions(Hero hero)
    {
        var options = statOptions.ToArray().Shuffled().Take(3).ToArray();
        return options
            .Select(o => new DraftStatLevelUpReward(o.description, () => hero.AddToStats(o.stats)))
            .Cast<LevelUpOption>()
            .ToArray();
    }
}
