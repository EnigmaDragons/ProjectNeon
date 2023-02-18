#if STEAMWORKS

using System;
using System.Collections.Generic;
using Steamworks;

public class SteamAchievements : IAchievements
{
    private readonly HashSet<string> _completedAchievements = new HashSet<string>();

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
        }
        catch (Exception e)
        {
            Log.Error(e);
            Log.Error($"Unable to Record Steam Achievement - {achievementId}");
        }
    }
}
#endif
