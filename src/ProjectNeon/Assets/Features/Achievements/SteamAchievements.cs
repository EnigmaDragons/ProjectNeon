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
        try
        {
            if (!SteamManager.Initialized)
                return;

            if (IsCompleted(achievementId))
                return;

            SteamUserStats.SetAchievement(achievementId);
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
        if (IsCompleted(Achievement.MiscAllAchievements))
            return;
        
        var numAchievements = SteamUserStats.GetNumAchievements();
        for (uint intId = 0; intId < numAchievements; ++intId)
        {
            var achievementId = SteamUserStats.GetAchievementName(intId);
            if (IsCompleted(achievementId))
                _completedAchievements.Add(achievementId);
            else
                return;
        }
        if (_completedAchievements.Count == numAchievements)
            Record(Achievement.MiscAllAchievements);
    }

    private bool IsCompleted(string achievementId)
    {
        if (_completedAchievements.Contains(achievementId))
            return true;
        
        SteamUserStats.GetAchievement(achievementId, out var isCompleted);
        if (isCompleted)
            _completedAchievements.Add(achievementId);

        return isCompleted;
    }
}
#endif
