#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemiesWantedEditor : EditorWindow
{
    [MenuItem("Neon/Enemies Wanted")]
    static void EnemiesWanted()
    {
        GetWindow(typeof(EnemiesWantedEditor)).Show();
    }

    private List<Range> Normals;
    private Vector2 scrollPos;

    void OnGUI()
    {
        if (GUILayout.Button("Refresh Normals"))
        {
            var encounterBuilderCalculator = GetAllInstances<EncounterBuilderToUseToCalculateWhatEnemiesAreNeeded>().First();
            var battleRoles = encounterBuilderCalculator.NormalEncounterBuilder._chancesBasedOnEnemyNumber.SelectMany(x => x.RollChances).Distinct().ToArray();
            var minTotal = 0;
            var maxTotal = 0;
            if (encounterBuilderCalculator.NormalPowerCurve is SimpleLinearPowerCurve)
            {
                var powerCurve = (SimpleLinearPowerCurve) encounterBuilderCalculator.NormalPowerCurve;
                minTotal = powerCurve._start;
                maxTotal = powerCurve._end;
            }
            if (encounterBuilderCalculator.NormalPowerCurve is MultiPointPowerCurve)
            {
                var powerCurve = (MultiPointPowerCurve) encounterBuilderCalculator.NormalPowerCurve;
                minTotal = powerCurve._start;
                maxTotal = powerCurve._end;
            }
            var min = Mathf.FloorToInt(encounterBuilderCalculator.NormalEncounterBuilder._weightedComps
                .Select(x => (float)x.Weights.Min() * ((float)minTotal / (float)x.Weights.Sum()))
                .OrderBy(x => x)
                .First());
            var max = Mathf.CeilToInt(encounterBuilderCalculator.NormalEncounterBuilder._weightedComps
                .Select(x => (float)x.Weights.Max() * ((float)maxTotal / (float)x.Weights.Sum()))
                .OrderByDescending(x => x)
                .First());
            var multiplier = (1 / (1 - encounterBuilderCalculator.NormalEncounterBuilder._flexibility)) * (1 + encounterBuilderCalculator.NormalEncounterBuilder._flexibility);
            var current = min;
            var ranges = new List<Range>();
            while (current < max)
            {
                ranges.Add(new Range { Min = current, Max = Mathf.FloorToInt(current * multiplier), EnemiesInRange = battleRoles.ToDictionary(x=> x, x => new List<Enemy>()) });
                current = ranges.Last().Max + 1;
            }
            foreach (var enemy in encounterBuilderCalculator.NormalEncounterBuilder._possible)
            {
                var range = ranges.FirstOrMaybe(x => x.Min <= enemy.stageDetails[0].powerLevel && x.Max >= enemy.stageDetails[0].powerLevel);
                if (range.IsMissing || !battleRoles.Contains(enemy.BattleRole))
                    continue;
                range.Value.EnemiesInRange[enemy.BattleRole].Add(enemy);
            }
            ranges.ForEach(x => x.EnemiesInRange = x.EnemiesInRange.ToDictionary(y => y.Key, y => y.Value.OrderBy(z => z.stageDetails[0].powerLevel).ToList()));
            Normals = ranges;
        }
        if (GUILayout.Button("Refresh Elites"))
        {
            var encounterBuilderCalculator = GetAllInstances<EncounterBuilderToUseToCalculateWhatEnemiesAreNeeded>().First();
            var battleRoles = encounterBuilderCalculator.EliteEncounterBuilder._chancesBasedOnEnemyNumber.SelectMany(x => x.RollChances).Distinct().ToArray();
            var minTotal = 0;
            var maxTotal = 0;
            if (encounterBuilderCalculator.ElitePowerCurve is SimpleLinearPowerCurve)
            {
                var powerCurve = (SimpleLinearPowerCurve) encounterBuilderCalculator.ElitePowerCurve;
                minTotal = powerCurve._start;
                maxTotal = powerCurve._end;
            }
            if (encounterBuilderCalculator.ElitePowerCurve is MultiPointPowerCurve)
            {
                var powerCurve = (MultiPointPowerCurve) encounterBuilderCalculator.ElitePowerCurve;
                minTotal = powerCurve._start;
                maxTotal = powerCurve._end;
            }
            var min = Mathf.FloorToInt(encounterBuilderCalculator.EliteEncounterBuilder._weightedComps
                .Select(x => (float)x.Weights.Max() * ((float)minTotal / (float)x.Weights.Sum()))
                .OrderBy(x => x)
                .First());
            var max = Mathf.CeilToInt(encounterBuilderCalculator.EliteEncounterBuilder._weightedComps
                .Select(x => (float)x.Weights.Max() * ((float)maxTotal / (float)x.Weights.Sum()))
                .OrderByDescending(x => x)
                .First());
            var multiplier = (1 / (1 - encounterBuilderCalculator.EliteEncounterBuilder._flexibility)) * (1 + encounterBuilderCalculator.EliteEncounterBuilder._flexibility);
            var current = min;
            var ranges = new List<Range>();
            while (current < max)
            {
                ranges.Add(new Range { Min = current, Max = Mathf.FloorToInt(current * multiplier), EnemiesInRange = battleRoles.ToDictionary(x=> x, x => new List<Enemy>()) });
                current = ranges.Last().Max + 1;
            }
            foreach (var enemy in encounterBuilderCalculator.EliteEncounterBuilder._possible.Where(x => x.Tier == EnemyTier.Elite))
            {
                var range = ranges.FirstOrMaybe(x => x.Min <= enemy.stageDetails[0].powerLevel && x.Max >= enemy.stageDetails[0].powerLevel);
                if (range.IsMissing || !battleRoles.Contains(enemy.BattleRole))
                    continue;
                range.Value.EnemiesInRange[enemy.BattleRole].Add(enemy);
            }
            ranges.ForEach(x => x.EnemiesInRange = x.EnemiesInRange.ToDictionary(y => y.Key, y => y.Value.OrderBy(z => z.stageDetails[0].powerLevel).ToList()));
            Normals = ranges;
        }
        if (Normals == null)
            return;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var range in Normals)
        {
            EditorGUILayout.LabelField($"Power Range: {range.Min} - {range.Max}");
            foreach (var enemyRoleInRange in range.EnemiesInRange)
            {
                EditorGUILayout.LabelField($"    Role: {enemyRoleInRange.Key}");
                foreach (var enemy in enemyRoleInRange.Value)
                {
                    EditorGUILayout.LabelField($"        Enemy: {enemy.name} - {enemy.stageDetails[0].powerLevel}");
                }
            }
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

    private class Range
    {
        public int Min;
        public int Max;
        public Dictionary<BattleRole, List<Enemy>> EnemiesInRange;
    }
}
#endif