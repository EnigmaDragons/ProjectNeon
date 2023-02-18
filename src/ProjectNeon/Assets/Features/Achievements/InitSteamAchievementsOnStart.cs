using UnityEngine;

public class InitSteamAchievementsOnStart : MonoBehaviour
{
    void Start()
    {
#if STEAMWORKS
        Achievements.Init(SteamAchievements.Create());
#else
        Achievements.Init(new NoAchievements());
#endif
    }
}
