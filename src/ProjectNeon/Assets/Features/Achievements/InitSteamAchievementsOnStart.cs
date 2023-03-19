using UnityEngine;

public class InitSteamAchievementsOnStart : MonoBehaviour
{
    void Start()
    {
#if STEAMWORKS
        this.ExecuteAfterDelay(() => Achievements.Init(SteamAchievements.Create()), 1f);
#else
        Achievements.Init(new NoAchievements());
#endif
    }
}
