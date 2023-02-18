#if STEAMWORKS

using System;
using System.Collections.Generic;
using Steamworks;

public class SteamAchievements : IAchievements
{
    private readonly HashSet<string> _completedAchievements = new HashSet<string>();

    public static SteamAchievements Create() => new SteamAchievements().Initialized();

    private SteamAchievements() { }
    
    private SteamAchievements Initialized()
    {
        var succeeded = SteamUserStats.RequestCurrentStats();
        if (!succeeded)
            Log.Warn("Unable to Get Current Steam Achievement Stats");
        return this;
    }
    
    public void Record(string achievementId)
    {
        if (_completedAchievements.Contains(achievementId))
            return;
        
        try
        {
            if (!SteamManager.Initialized)
                return;
            
            SteamUserStats.GetAchievement(achievementId, out var isRecordedAlready);
            if (!isRecordedAlready)
                SteamUserStats.SetAchievement(achievementId);
            else
                _completedAchievements.Add(achievementId);
            CheckForAllAchievementsCompleted();
        }
        catch (Exception e)
        {
            Log.Error(e);
            Log.Error($"Unable to Record Steam Achievement - {achievementId}");
        }
    }

    private void CheckForAllAchievementsCompleted()
    {
        var numAchievements = SteamUserStats.GetNumAchievements();
        for (uint intId = 0; intId < numAchievements; ++intId)
        {
            var achievementId = SteamUserStats.GetAchievementName(intId);
            SteamUserStats.GetAchievement(achievementId, out var isRecordedAlready);
            if (isRecordedAlready)
                _completedAchievements.Add(achievementId);
            else
                return;
        }
        if (_completedAchievements.Count == numAchievements)
            Record(Achievement.MiscAllAchievements);
    }
}
#endif
