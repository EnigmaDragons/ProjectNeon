#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyStatsEditorTemp : EditorWindow
{
    [MenuItem("Neon/Enemy Stats")]
    static void EnemyStats()
    {
        GetWindow(typeof(EnemyStatsEditorTemp)).Show();
    }

    private bool initialized;
    private Enemy[] enemies;
    private int totalHealth;
    private decimal averageHealth;
    private int totalShields;
    private int enemiesWithShields;
    private decimal percentageOfEnemiesWithShields;
    private int totalArmor;
    private int enemiesWithArmor;
    private decimal percentageOfEnemiesWithArmor;
    private int totalResistance;
    private int enemiesWithResistance;
    private decimal percentageOfEnemiesWithResistance;
    private int totalDodge;
    private int enemiesWithDodge;
    private decimal percentageOfEnemiesWithDodge;
    private int totalAegis;
    private int enemiesWithAegis;
    private decimal percentageOfEnemiesWithAegis;
    private Vector2 scrollPos;
    
    void OnGUI()
    {
        if (!initialized)
        {
            enemies = GetAllInstances<Enemy>().Where(x => !x.ExcludeFromBestiary && x.stageDetails.Length == 1).OrderBy(x => x.stageDetails[0].powerLevel).ToArray();
            var totalEnemies = enemies.Length;
            totalHealth = enemies.Sum(x => x.stageDetails[0].maxHp);
            averageHealth = (decimal)totalHealth / totalEnemies;
            totalShields = enemies.Sum(x => x.stageDetails[0].startingShield);
            enemiesWithShields = enemies.Count(x => x.stageDetails[0].startingShield > 0);
            percentageOfEnemiesWithShields = (decimal)enemiesWithShields / totalEnemies * 100;
            totalArmor = enemies.Sum(x => x.stageDetails[0].armor);
            enemiesWithArmor = enemies.Count(x => x.stageDetails[0].armor > 0);
            percentageOfEnemiesWithArmor = (decimal)enemiesWithArmor / totalEnemies * 100;
            totalResistance = enemies.Sum(x => x.stageDetails[0].resistance);
            enemiesWithResistance = enemies.Count(x => x.stageDetails[0].resistance > 0);
            percentageOfEnemiesWithResistance = (decimal)enemiesWithResistance / totalEnemies * 100;
            totalDodge = enemies.Sum(x => x.stageDetails[0].startingDodge);
            enemiesWithDodge = enemies.Count(x => x.stageDetails[0].startingDodge > 0);
            percentageOfEnemiesWithDodge = (decimal)enemiesWithDodge / totalEnemies * 100;
            totalAegis = enemies.Sum(x => x.stageDetails[0].startingAegis);
            enemiesWithAegis = enemies.Count(x => x.stageDetails[0].startingAegis > 0);
            percentageOfEnemiesWithAegis = (decimal)enemiesWithAegis / totalEnemies * 100;
            initialized = true;
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.LabelField($"Enemies: {enemies.Length}");
        EditorGUILayout.LabelField($"Health: Total = {totalHealth} | Average = {averageHealth:0.##}");
        EditorGUILayout.LabelField($"Shields: Total = {totalShields} | Enemies = {enemiesWithShields} | Percentage = {percentageOfEnemiesWithShields:0.##}%");
        EditorGUILayout.LabelField($"Armor: Total = {totalArmor} | Enemies = {enemiesWithArmor} | Percentage = {percentageOfEnemiesWithArmor:0.##}%");
        EditorGUILayout.LabelField($"Resistance: Total = {totalResistance} | Enemies = {enemiesWithResistance} | Percentage = {percentageOfEnemiesWithResistance:0.##}%");
        EditorGUILayout.LabelField($"Dodge: Total = {totalDodge} | Enemies = {enemiesWithDodge} | Percentage = {percentageOfEnemiesWithDodge:0.##}%");
        EditorGUILayout.LabelField($"Aegis: Total = {totalAegis} | Enemies = {enemiesWithAegis} | Percentage = {percentageOfEnemiesWithAegis:0.##}%");
        DrawUILine();
        foreach (var enemy in enemies)
        {
            EditorGUILayout.LabelField($"Power: {enemy.stageDetails[0].powerLevel}");
        }
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