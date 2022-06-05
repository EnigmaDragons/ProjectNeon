using UnityEditor;

public class CustomRebalanceScript
{
    [MenuItem("Neon/Custom Rebalance")]
    public static void Go()
    {
        RebalanceHp();
    }

    [MenuItem("Neon/Revert Last Rebalance")]
    public static void Revert()
    {
        RevertLastHpRebalance();
    }
    

    private static void RebalanceHp()
    {        
        ScriptableExtensions.GetAllInstances<BaseHero>().ForEach(h =>
        {
            h.maxHp = (h.maxHp * 1.1f).CeilingInt();
            EditorUtility.SetDirty(h);
        });
        ScriptableExtensions.GetAllInstances<Enemy>().ForEach(e =>
        {
            if (e.Tier == EnemyTier.Minion || e.Tier == EnemyTier.Boss)
                return;

            var amountToIncrease = (e.stageDetails[0].powerLevel * 0.12f).CeilingInt();
            e.stageDetails[0].maxHp += amountToIncrease;
            EditorUtility.SetDirty(e);
        });
    }

    private static void RevertLastHpRebalance()
    {
        ScriptableExtensions.GetAllInstances<BaseHero>().ForEach(h =>
        {
            h.maxHp = (h.maxHp / 1.1f).CeilingInt();
            EditorUtility.SetDirty(h);
        });
        
        ScriptableExtensions.GetAllInstances<Enemy>().ForEach(e =>
        {
            if (e.Tier == EnemyTier.Minion || e.Tier == EnemyTier.Boss)
                return;

            var amountToIncrease = (e.stageDetails[0].powerLevel * 0.12f).CeilingInt();
            e.stageDetails[0].maxHp -= amountToIncrease;
            EditorUtility.SetDirty(e);
        });
    }
}
