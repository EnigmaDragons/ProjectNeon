
public interface IAchievements
{
    void Record(string achievementId);
}

public class NoAchievements : IAchievements
{
    public void Record(string achievementId) {}
}
