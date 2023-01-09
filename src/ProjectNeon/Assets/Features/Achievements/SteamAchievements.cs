#if STEAMWORKS

using System;
using Steamworks;

public class SteamAchievements : IAchievements
{
    public void Record(string achievementId)
    {
        try
        {
            SteamUserStats.GetAchievement(achievementId, out var isRecordedAlready);
            if (!isRecordedAlready)
                SteamUserStats.SetAchievement(achievementId);
        }
        catch (Exception e)
        {
            Log.Error(e);
            Log.Error($"Unable to Record Steam Achievement - {achievementId}");
        }
    }
}
#endif
