#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyStatsEditor : EditorWindow
{
    [MenuItem("Neon/Enemy Stats")]
    static void EnemyStats()
    {
        GetWindow(typeof(EnemyStatsEditor)).Show();
    }

    private bool initialized;
    private Enemy[] normalEnemies;
    private int totalNormalEnemies;
    private int totalNormalEnemiesHealth;
    private decimal averageNormalEnemyHealth;
    private int totalNormalEnemyShields;
    private int normalEnemiesWithShields;
    private decimal percentageOfNormalEnemiesWithShields;
    private int totalNormalEnemyArmor;
    private int normalEnemiesWithArmor;
    private decimal percentageOfNormalEnemiesWithArmor;
    private int totalNormalEnemyResistance;
    private int normalEnemiesWithResistance;
    private decimal percentageOfNormalEnemiesWithResistance;
    private int totalNormalEnemyDodge;
    private int normalEnemiesWithDodge;
    private decimal percentageOfNormalEnemiesWithDodge;
    private int totalNormalEnemyAegis;
    private int normalEnemiesWithAegis;
    private decimal percentageOfNormalEnemiesWithAegis;
    private int normalDamageDealers;
    private decimal percentageOfNormalDamageDealers;
    private int normalDamageMitigators;
    private decimal percentageOfNormalDamageMitigators;
    private int normalSpecialists;
    private decimal percentageOfNormalSpecialists;
    private Enemy[] eliteEnemies;
    private int totalEliteEnemies;
    private int totalEliteEnemiesHealth;
    private decimal averageEliteEnemyHealth;
    private int totalEliteEnemyShields;
    private int eliteEnemiesWithShields;
    private decimal percentageOfEliteEnemiesWithShields;
    private int totalEliteEnemyArmor;
    private int eliteEnemiesWithArmor;
    private decimal percentageOfEliteEnemiesWithArmor;
    private int totalEliteEnemyResistance;
    private int eliteEnemiesWithResistance;
    private decimal percentageOfEliteEnemiesWithResistance;
    private int totalEliteEnemyDodge;
    private int eliteEnemiesWithDodge;
    private decimal percentageOfEliteEnemiesWithDodge;
    private int totalEliteEnemyAegis;
    private int eliteEnemiesWithAegis;
    private decimal percentageOfEliteEnemiesWithAegis;
    private int eliteDamageDealers;
    private decimal percentageOfEliteDamageDealers;
    private int eliteDamageMitigators;
    private decimal percentageOfEliteDamageMitigators;
    private int eliteSpecialists;
    private decimal percentageOfEliteSpecialists;
    private Vector2 scrollPos;
    
    void OnGUI()
    {
        if (!initialized)
        {
            var enemies = GetAllInstances<Enemy>().Where(x => !x.ExcludeFromBestiary && x.stageDetails.Length == 1).OrderBy(x => x.stageDetails[0].powerLevel).ToArray();
            normalEnemies = enemies.Where(x => x.Tier == EnemyTier.Minion || x.Tier == EnemyTier.Normal).ToArray();
            totalNormalEnemies = normalEnemies.Length;
            totalNormalEnemiesHealth = normalEnemies.Sum(x => x.stageDetails[0].maxHp);
            averageNormalEnemyHealth = (decimal)totalNormalEnemiesHealth / totalNormalEnemies;
            totalNormalEnemyShields = normalEnemies.Sum(x => x.stageDetails[0].startingShield);
            normalEnemiesWithShields = normalEnemies.Count(x => x.stageDetails[0].startingShield > 0);
            percentageOfNormalEnemiesWithShields = (decimal)normalEnemiesWithShields / totalNormalEnemies * 100;
            totalNormalEnemyArmor = normalEnemies.Sum(x => x.stageDetails[0].armor);
            normalEnemiesWithArmor = normalEnemies.Count(x => x.stageDetails[0].armor > 0);
            percentageOfNormalEnemiesWithArmor = (decimal)normalEnemiesWithArmor / totalNormalEnemies * 100;
            totalNormalEnemyResistance = normalEnemies.Sum(x => x.stageDetails[0].resistance);
            normalEnemiesWithResistance = normalEnemies.Count(x => x.stageDetails[0].resistance > 0);
            percentageOfNormalEnemiesWithResistance = (decimal)normalEnemiesWithResistance / totalNormalEnemies * 100;
            totalNormalEnemyDodge = normalEnemies.Sum(x => x.stageDetails[0].startingDodge);
            normalEnemiesWithDodge = normalEnemies.Count(x => x.stageDetails[0].startingDodge > 0);
            percentageOfNormalEnemiesWithDodge = (decimal)normalEnemiesWithDodge / totalNormalEnemies * 100;
            totalNormalEnemyAegis = normalEnemies.Sum(x => x.stageDetails[0].startingAegis);
            normalEnemiesWithAegis = normalEnemies.Count(x => x.stageDetails[0].startingAegis > 0);
            percentageOfNormalEnemiesWithAegis = (decimal)normalEnemiesWithAegis / totalNormalEnemies * 100;
            normalDamageDealers = normalEnemies.Count(x => x.BattleRole == BattleRole.DamageDealer);
            percentageOfNormalDamageDealers = (decimal)normalDamageDealers / totalNormalEnemies * 100;
            normalDamageMitigators = normalEnemies.Count(x => x.BattleRole == BattleRole.Survivability);
            percentageOfNormalDamageMitigators = (decimal)normalDamageMitigators / totalNormalEnemies * 100;
            normalSpecialists = normalEnemies.Count(x => x.BattleRole == BattleRole.Specialist);
            percentageOfNormalSpecialists = (decimal)normalSpecialists / totalNormalEnemies * 100;
            eliteEnemies = enemies.Where(x => x.Tier == EnemyTier.Elite).ToArray();
            totalEliteEnemies = eliteEnemies.Length;
            totalEliteEnemiesHealth = eliteEnemies.Sum(x => x.stageDetails[0].maxHp);
            averageEliteEnemyHealth = (decimal)totalEliteEnemiesHealth / totalEliteEnemies;
            totalEliteEnemyShields = eliteEnemies.Sum(x => x.stageDetails[0].startingShield);
            eliteEnemiesWithShields = eliteEnemies.Count(x => x.stageDetails[0].startingShield > 0);
            percentageOfEliteEnemiesWithShields = (decimal)eliteEnemiesWithShields / totalEliteEnemies * 100;
            totalEliteEnemyArmor = eliteEnemies.Sum(x => x.stageDetails[0].armor);
            eliteEnemiesWithArmor = eliteEnemies.Count(x => x.stageDetails[0].armor > 0);
            percentageOfEliteEnemiesWithArmor = (decimal)eliteEnemiesWithArmor / totalEliteEnemies * 100;
            totalEliteEnemyResistance = eliteEnemies.Sum(x => x.stageDetails[0].resistance);
            eliteEnemiesWithResistance = eliteEnemies.Count(x => x.stageDetails[0].resistance > 0);
            percentageOfEliteEnemiesWithResistance = (decimal)eliteEnemiesWithResistance / totalEliteEnemies * 100;
            totalEliteEnemyDodge = eliteEnemies.Sum(x => x.stageDetails[0].startingDodge);
            eliteEnemiesWithDodge = eliteEnemies.Count(x => x.stageDetails[0].startingDodge > 0);
            percentageOfEliteEnemiesWithDodge = (decimal)eliteEnemiesWithDodge / totalEliteEnemies * 100;
            totalEliteEnemyAegis = eliteEnemies.Sum(x => x.stageDetails[0].startingAegis);
            eliteEnemiesWithAegis = eliteEnemies.Count(x => x.stageDetails[0].startingAegis > 0);
            percentageOfEliteEnemiesWithAegis = (decimal)eliteEnemiesWithAegis / totalEliteEnemies * 100;
            eliteDamageDealers = eliteEnemies.Count(x => x.BattleRole == BattleRole.DamageDealer);
            percentageOfEliteDamageDealers = (decimal)eliteDamageDealers / totalEliteEnemies * 100;
            eliteDamageMitigators = eliteEnemies.Count(x => x.BattleRole == BattleRole.Survivability);
            percentageOfEliteDamageMitigators = (decimal)eliteDamageMitigators / totalEliteEnemies * 100;
            eliteSpecialists = eliteEnemies.Count(x => x.BattleRole == BattleRole.Specialist);
            percentageOfEliteSpecialists = (decimal)eliteSpecialists / totalEliteEnemies * 100;
            initialized = true;
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.LabelField($"Enemies:");
        EditorGUILayout.LabelField($"    Normals = {totalNormalEnemies}");
        EditorGUILayout.LabelField($"    Elites = {totalEliteEnemies}");
        EditorGUILayout.LabelField($"Health:");
        EditorGUILayout.LabelField($"    Total Normal Enemy = {totalNormalEnemiesHealth}");
        EditorGUILayout.LabelField($"    Average Normal Enemy = {averageNormalEnemyHealth:0.##}");
        EditorGUILayout.LabelField($"    Total Elite Enemy = {totalEliteEnemiesHealth}");
        EditorGUILayout.LabelField($"    Average Elite Enemy = {averageEliteEnemyHealth:0.##}");
        EditorGUILayout.LabelField($"Shields:");
        EditorGUILayout.LabelField($"   Total Normal Enemy = {totalNormalEnemyShields}");
        EditorGUILayout.LabelField($"   Normal Enemies = {normalEnemiesWithShields}");
        EditorGUILayout.LabelField($"   Normal Enemy Percentage = {percentageOfNormalEnemiesWithShields:0.##}%");
        EditorGUILayout.LabelField($"   Total Elite Enemy = {totalEliteEnemyShields}");
        EditorGUILayout.LabelField($"   Elite Enemies = {eliteEnemiesWithShields}");
        EditorGUILayout.LabelField($"   Elite Enemy Percentage = {percentageOfEliteEnemiesWithShields:0.##}%");
        EditorGUILayout.LabelField($"Armor:");
        EditorGUILayout.LabelField($"   Total Normal Enemy = {totalNormalEnemyArmor}");
        EditorGUILayout.LabelField($"   Normal Enemies = {normalEnemiesWithArmor}");
        EditorGUILayout.LabelField($"   Normal Enemy Percentage = {percentageOfNormalEnemiesWithArmor:0.##}%");
        EditorGUILayout.LabelField($"   Total Elite Enemy = {totalEliteEnemyArmor}");
        EditorGUILayout.LabelField($"   Elite Enemies = {eliteEnemiesWithArmor}");
        EditorGUILayout.LabelField($"   Elite Enemy Percentage = {percentageOfEliteEnemiesWithArmor:0.##}%");
        EditorGUILayout.LabelField($"Resistance:");
        EditorGUILayout.LabelField($"   Total Normal Enemy = {totalNormalEnemyResistance}");
        EditorGUILayout.LabelField($"   Normal Enemies = {normalEnemiesWithResistance}");
        EditorGUILayout.LabelField($"   Normal Enemy Percentage = {percentageOfNormalEnemiesWithResistance:0.##}%");
        EditorGUILayout.LabelField($"   Total Elite Enemy = {totalEliteEnemyResistance}");
        EditorGUILayout.LabelField($"   Elite Enemies = {eliteEnemiesWithResistance}");
        EditorGUILayout.LabelField($"   Elite Enemy Percentage = {percentageOfEliteEnemiesWithResistance:0.##}%");
        EditorGUILayout.LabelField($"Dodge:");
        EditorGUILayout.LabelField($"   Total Normal Enemy = {totalNormalEnemyDodge}");
        EditorGUILayout.LabelField($"   Normal Enemies = {normalEnemiesWithDodge}");
        EditorGUILayout.LabelField($"   Normal Enemy Percentage = {percentageOfNormalEnemiesWithDodge:0.##}%");
        EditorGUILayout.LabelField($"   Total Elite Enemy = {totalEliteEnemyDodge}");
        EditorGUILayout.LabelField($"   Elite Enemies = {eliteEnemiesWithDodge}");
        EditorGUILayout.LabelField($"   Elite Enemy Percentage = {percentageOfEliteEnemiesWithDodge:0.##}%");
        EditorGUILayout.LabelField($"Aegis:");
        EditorGUILayout.LabelField($"   Total Normal Enemy = {totalNormalEnemyAegis}");
        EditorGUILayout.LabelField($"   Normal Enemies = {normalEnemiesWithAegis}");
        EditorGUILayout.LabelField($"   Normal Enemy Percentage = {percentageOfNormalEnemiesWithAegis:0.##}%");
        EditorGUILayout.LabelField($"   Total Elite Enemy = {totalEliteEnemyAegis}");
        EditorGUILayout.LabelField($"   Elite Enemies = {eliteEnemiesWithAegis}");
        EditorGUILayout.LabelField($"   Elite Enemy Percentage = {percentageOfEliteEnemiesWithAegis:0.##}%");
        EditorGUILayout.LabelField($"Roles:");
        EditorGUILayout.LabelField($"   Normal Damage Dealers = {normalDamageDealers}");
        EditorGUILayout.LabelField($"   Normal Damage Dealer Percentage = {percentageOfNormalDamageDealers:0.##}%");
        EditorGUILayout.LabelField($"   Normal Damage Mitigators = {normalDamageMitigators}");
        EditorGUILayout.LabelField($"   Normal Damage Mitigator Percentage = {percentageOfNormalDamageMitigators:0.##}%");
        EditorGUILayout.LabelField($"   Normal Specialists = {normalSpecialists}");
        EditorGUILayout.LabelField($"   Normal Specialist Percentage = {percentageOfNormalSpecialists:0.##}%");
        EditorGUILayout.LabelField($"   Elite Damage Dealers = {eliteDamageDealers}");
        EditorGUILayout.LabelField($"   Elite Damage Dealer Percentage = {percentageOfEliteDamageDealers:0.##}%");
        EditorGUILayout.LabelField($"   Elite Damage Mitigators = {eliteDamageMitigators}");
        EditorGUILayout.LabelField($"   Elite Damage Mitigator Percentage = {percentageOfEliteDamageMitigators:0.##}%");
        EditorGUILayout.LabelField($"   Elite Specialists = {eliteSpecialists}");
        EditorGUILayout.LabelField($"   Elite Specialist Percentage = {percentageOfEliteSpecialists:0.##}%");
        DrawUILine();
        EditorGUILayout.LabelField($"Normal Enemies:");
        foreach (var enemy in normalEnemies)
            EditorGUILayout.LabelField($"    {enemy.enemyName}: Power = {enemy.stageDetails[0].powerLevel} | {enemy.BattleRole}");
        DrawUILine();
        EditorGUILayout.LabelField($"Elite Enemies:");
        foreach (var enemy in eliteEnemies)
            EditorGUILayout.LabelField($"    {enemy.enemyName}: Power = {enemy.stageDetails[0].powerLevel} | {enemy.BattleRole}");
        EditorGUILayout.EndScrollView();
    }
    
    private static T[] GetAllInstances<T>() where T : ScriptableObject 
        => AssetDatabase.FindAssets("t:" + typeof(T).Name)
            .Select(x => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(x))).ToArray();
    
    private void DrawUILine() => DrawUILine(Color.black);
    private void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
    }
}
#endif