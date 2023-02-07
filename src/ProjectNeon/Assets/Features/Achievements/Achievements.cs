
public static class Achievements
{
    private static IAchievements Instance = new NoAchievements();

    public static void Record(string achievementId) => Instance.Record(achievementId);

    public static void Init(IAchievements instance) => Instance = instance;

    public static void RecordAdventureCompleted(int adventureId, bool wasVictorious, Difficulty difficulty, string[] englishHeroNames)
    {
        const int breakIntoMetroplexZeroAdventureId = 10;
        const int organizedHarvestorsAdventureId = 9;
        const int antiRobotSentimentsAdventureId = 14;

        if (adventureId == breakIntoMetroplexZeroAdventureId)
            if (wasVictorious)
                Record(Achievement.AdventureWonBreakIntoMetroplexZero);
            else
                Record(Achievement.AdventureLostBreakIntoMetroplexZero);

        if (adventureId == organizedHarvestorsAdventureId)
            if (wasVictorious)
                Record(Achievement.AdventureWonOrganizedHarvestors);
            else
                Record(Achievement.AdventureLostOrganizedHarvestors);

        if (adventureId == antiRobotSentimentsAdventureId && wasVictorious)
            Record(Achievement.AdventureWonAntiRobotSentiments);

        if (wasVictorious)
        {
            if (difficulty.Id == -1)
                Record(Achievement.DifficultyCasual);
            if (difficulty.Id == 1)
                Record(Achievement.DifficultyVeteran);
            if (difficulty.Id == 2)
                Record(Achievement.DifficultyIllegalTech);
            if (difficulty.Id == 3)
                Record(Achievement.DifficultyPromotions);
            if (difficulty.Id == 4)
                Record(Achievement.DifficultyOppression);
            if (difficulty.Id == 5)
                Record(Achievement.DifficultyDystopia);

            foreach (var h in englishHeroNames)
            {
                Record(Achievement.HeroVictory(h));
                if (difficulty.Id == 5)
                    Record(Achievement.HeroMasterVictory(h));
            }
        }
    }
}

