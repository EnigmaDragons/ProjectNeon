
using System.Collections.Generic;

public static class Achievements
{
    private static IAchievements Instance = new NoAchievements();

    public static void Record(string achievementId) => Instance.Record(achievementId);

    public static void Init(IAchievements instance) => Instance = instance;

    public static void RecordAdventureCompleted(int adventureId, bool wasVictorious, Difficulty difficulty, string[] englishHeroNames, HashSet<string> storyStates)
    {
        // Core Adventures
        if (adventureId == AdventureIds.BreakIntoMetroplexZeroAdventureId)
            if (wasVictorious)
                Record(Achievement.AdventureWonBreakIntoMetroplexZero);
            else
                Record(Achievement.AdventureLostBreakIntoMetroplexZero);

        if (adventureId == AdventureIds.OrganizedHarvestorsAdventureId)
            if (wasVictorious)
                Record(Achievement.AdventureWonOrganizedHarvestors);
            else
                Record(Achievement.AdventureLostOrganizedHarvestors);

        if (adventureId == AdventureIds.AntiRobotSentimentsAdventureId && wasVictorious)
            Record(Achievement.AdventureWonAntiRobotSentiments);

        // Draft Adventures
        if (wasVictorious && adventureId == AdventureIds.SoloDraftId)
            Record(Achievement.AdventureWonSoloDraft);
        if (wasVictorious && adventureId == AdventureIds.DuoDraftId)
            Record(Achievement.AdventureWonDuoDraft);
        if (wasVictorious && adventureId == AdventureIds.TrioDraftId)
            Record(Achievement.AdventureWonTrioDraft);
        
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
            
            if (adventureId == AdventureIds.OrganizedHarvestorsAdventureId)
                if (storyStates.Contains("AntonDies"))
                    Record(Achievement.EndingAntonKilled);
                else if (storyStates.Contains("LetAntonLive"))
                    Record(Achievement.EndingAntonLive);
                else if (storyStates.Contains("VirusActivated"))
                    Record(Achievement.EndingAntonVirus);
            
            if (adventureId == AdventureIds.AntiRobotSentimentsAdventureId)
                if (storyStates.Contains("MimicAssistance"))
                    Record(Achievement.EndingMimicsAllied);
                else if (storyStates.Contains("MimicExile"))
                    Record(Achievement.EndingMimicsExiled);
                else if (storyStates.Contains("MimicTermination"))
                    Record(Achievement.EndingMimicsTerminated);
                else if (storyStates.Contains("MimicControl"))
                    Record(Achievement.EndingMimicsEnslaved);

            foreach (var h in englishHeroNames)
            {
                Record(Achievement.HeroVictory(h));
                if (difficulty.Id == 5)
                    Record(Achievement.HeroMasterVictory(h));
            }
        }
    }
}

