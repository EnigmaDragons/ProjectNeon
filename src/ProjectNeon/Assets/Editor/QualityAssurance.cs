using UnityEditor;

public class QualityAssurance
{
    [MenuItem("Neon/Run Quality Assurance Step")]
    public static void Go()
    {
        Log.Info("QA - Started");
        QaAllEnemies();
    }

    private static void QaAllEnemies()
    {
        var enemies = ScriptableExtensions.GetAllInstances<Enemy>();
        var numberEnemiesBad = 0;
        var numEnemiesCount = enemies.Length;
        foreach (var e in enemies)
        {
            if (e.IsCurrentlyWorking && !e.IsReadyForPlay)
            {
                Log.Error($"Broken Enemy: {e.EnemyName}");
                ++numberEnemiesBad;
            }
        }
        Log.Info($"QA - Enemies: {numEnemiesCount - numberEnemiesBad} out of {numEnemiesCount} are working.");
    }
}
