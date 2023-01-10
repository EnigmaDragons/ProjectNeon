
public static class Achievements
{
    private static IAchievements Instance = new NoAchievements();

    public static void Record(string achievementId) => Instance.Record(achievementId);

    public static void Init(IAchievements instance) => Instance = instance;
    
    public static void RecordAdventureCompleted(int adventureId, bool wasVictorious)
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
    }
}

