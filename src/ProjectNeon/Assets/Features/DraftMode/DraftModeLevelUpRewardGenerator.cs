using System;
using System.Linq;

public static class DraftModeLevelUpRewardGenerator
{
    private static (Func<string> description, StatAddends stats)[] statOptions =
    {
        (() => $"+8 {"Stats/Stat-MaxHP".ToLocalized()}", new StatAddends().With(StatType.MaxHP, 8)),
        (() => $"+1 {"Stats/Stat-Armor".ToLocalized()}", new StatAddends().With(StatType.Armor, 1)),
        (() => $"+1 {"Stats/Stat-Resistance".ToLocalized()}", new StatAddends().With(StatType.Resistance, 1)),
        (() => $"+1 {"Stats/Stat-Power".ToLocalized()}", new StatAddends().With(StatType.Power, 1)),
        (() => $"+16 {"Stats/Stat-MaxShield".ToLocalized()}", new StatAddends().With(StatType.MaxShield, 16)),
        (() => $"+4 {"Stats/Stat-StartingShield".ToLocalized()}", new StatAddends().With(StatType.StartingShield, 4)),
    };
    
    public static LevelUpOption[] GenerateOptions(Hero hero)
    {
        var options = statOptions.ToArray().Shuffled().Take(3).ToArray();
        return options
            .Select(o => new DraftStatLevelUpReward(o.description(), () => hero.AddToStats(o.stats)))
            .Cast<LevelUpOption>()
            .ToArray();
    }
}
