using UnityEngine;

public class InitSteamAchievementsOnStart : MonoBehaviour
{
    void Start()
    {
#if STEAMWORKS_NET
        Achievements.Init(new SteamAchievements());
#else
        Achievements.Init(new NoAchievements());
#endif
    }
}
