using System.Linq;

public static class DraftModeLevelUpRewardGenerator
{
    private static (string description, StatAddends stats)[] statOptions =
    {
        ("+6 MaxHP", new StatAddends().With(StatType.MaxHP, 6)),
        ("+2 Armor", new StatAddends().With(StatType.Armor, 1)),
        ("+2 Resistance", new StatAddends().With(StatType.Resistance, 1)),
        ("+1 Power", new StatAddends().With(StatType.Power, 1)),
        ("+10 Max Shield", new StatAddends().With(StatType.MaxShield, 10)),
        ("+3 Starting Shield", new StatAddends().With(StatType.StartingShield, 10)),
    };
    
    public static LevelUpOption[] GenerateOptions(Hero hero, PartyAdventureState party)
    {
        var options = statOptions.ToArray().Shuffled().Take(3).ToArray();
        return options
            .Select(o => new DraftStatLevelUpReward(o.description, () => hero.AddToStats(o.stats)))
            .Cast<LevelUpOption>()
            .ToArray();
    }
}
