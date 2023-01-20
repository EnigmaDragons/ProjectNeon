using UnityEngine;

public class InitSteamAchievementsOnStart : MonoBehaviour
{
    void Start()
    {
#if STEAMWORKS
        Achievements.Init(new SteamAchievements());
#else
        Achievements.Init(new NoAchievements());
#endif
    }
}
