using System;
using System.Collections.Generic;
using System.Linq;

public static class DraftModeLevelUpRewardGenerator
{
    private static Dictionary<StatType, (Func<string> description, StatAddends stats)> statOptions = new Dictionary<StatType,(Func<string> description, StatAddends stats)>
    {
        { StatType.MaxHP, (() => $"+8 {"Stats/Stat-MaxHP".ToLocalized()}", new StatAddends().With(StatType.MaxHP, 8)) },
        { StatType.Armor, (() => $"+1 {"Stats/Stat-Armor".ToLocalized()}", new StatAddends().With(StatType.Armor, 1)) },
        { StatType.Resistance, (() => $"+1 {"Stats/Stat-Resistance".ToLocalized()}", new StatAddends().With(StatType.Resistance, 1)) },
        { StatType.Power, (() => $"+1 {"Stats/Stat-Power".ToLocalized()}", new StatAddends().With(StatType.Power, 1)) },
        { StatType.MaxShield, (() => $"+16 {"Stats/Stat-MaxShield".ToLocalized()}", new StatAddends().With(StatType.MaxShield, 16)) },
        { StatType.StartingShield, (() => $"+4 {"Stats/Stat-StartingShield".ToLocalized()}", new StatAddends().With(StatType.StartingShield, 4)) },
    };

    public static StatType[] GenerateOptions() => statOptions.Keys.ToArray().Shuffled().Take(3).ToArray();

    public static LevelUpOption CreateLevelUpOption(Hero hero, StatType stat)
    {
        var option = statOptions[stat];
        return new DraftStatLevelUpReward(option.description(), () => hero.AddToStats(option.stats));
    }
}
