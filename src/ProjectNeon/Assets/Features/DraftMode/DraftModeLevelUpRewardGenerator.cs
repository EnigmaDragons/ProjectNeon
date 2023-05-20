using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public static class DraftModeLevelUpRewardGenerator
{
    private class DescriptionsGenerator
    {
        public Func<string> GetDescription { get; set;  }
        public Func<string> GetEnglishDescription { get; set;  }
        public StatAddends Stats { get; set;  }

        public static DescriptionsGenerator Create(Func<string> getDesc, Func<string> getEnglish, StatAddends stats) => new DescriptionsGenerator
        {
            GetDescription = getDesc,
            GetEnglishDescription = getEnglish,
            Stats = stats
        };
    }

    private static Dictionary<StatType, DescriptionsGenerator> statOptions = new Dictionary<StatType, DescriptionsGenerator>
    {
        { StatType.MaxHP, DescriptionsGenerator.Create(() => $"+8 {"Stats/Stat-MaxHP".ToLocalized()}", () => $"+8 {"Stats/Stat-MaxHP".ToEnglish()}", new StatAddends().With(StatType.MaxHP, 8)) },
        { StatType.Armor, DescriptionsGenerator.Create(() => $"+1 {"Stats/Stat-Armor".ToLocalized()}", () => $"+1 {"Stats/Stat-Armor".ToEnglish()}", new StatAddends().With(StatType.Armor, 1)) },
        { StatType.Resistance, DescriptionsGenerator.Create(() => $"+1 {"Stats/Stat-Resistance".ToLocalized()}", () => $"+1 {"Stats/Stat-Resistance".ToEnglish()}", new StatAddends().With(StatType.Resistance, 1)) },
        { StatType.Power, DescriptionsGenerator.Create(() => $"+1 {"Stats/Stat-Power".ToLocalized()}", () => $"+1 {"Stats/Stat-Power".ToEnglish()}", new StatAddends().With(StatType.Power, 1)) },
        { StatType.MaxShield, DescriptionsGenerator.Create(() => $"+16 {"Stats/Stat-MaxShield".ToLocalized()}", () => $"+16 {"Stats/Stat-MaxShield".ToEnglish()}", new StatAddends().With(StatType.MaxShield, 16)) },
        { StatType.StartingShield, DescriptionsGenerator.Create(() => $"+4 {"Stats/Stat-StartingShield".ToLocalized()}", () => $"+4 {"Stats/Stat-StartingShield".ToEnglish()}", new StatAddends().With(StatType.StartingShield, 4)) },
    };

    public static StatType[] GenerateOptions() => statOptions.Keys.ToArray().Shuffled().Take(3).ToArray();

    public static LevelUpOption CreateLevelUpOption(Hero hero, StatType stat)
    {
        var option = statOptions[stat];
        return new DraftStatLevelUpReward(option.GetDescription(), option.GetEnglishDescription(), () => hero.AddToStats(option.Stats));
    }
}
