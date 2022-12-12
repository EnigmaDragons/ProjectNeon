
public static class Achievements
{
    private static IAchievements Instance = new NoAchievements();

    private static void Record(string achievementId) => Instance.Record(achievementId);

    public static void Init(IAchievements instance) => Instance = instance;
}

