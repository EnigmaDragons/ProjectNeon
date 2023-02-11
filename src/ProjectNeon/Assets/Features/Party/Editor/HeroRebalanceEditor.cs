#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public class HeroRebalanceEditor : EditorWindow
{
    [MenuItem("Neon/Rebalance Heroes")]
    static void SelectGameObjects()
    {
        GetWindow(typeof(HeroRebalanceEditor)).Show();
    }

    private float _baseHpFactor = 1f;
    private float _growthHpFactor = 1f;
    
    void OnGUI()
    {
        _baseHpFactor = EditorGUILayout.FloatField("Base HP Factor", _baseHpFactor);
        if (GUILayout.Button("Rebalance Base HP")) 
        {
            Log.Info("Rebalance Heroes Begun");
            var heroes =  UnityResourceUtils.FindAssetsByType<BaseHero>()
                .Where(h => !h.name.Equals("Anon"))
                .ToArray();
            
            heroes.ForEach(h =>
            {
                h.maxHp = (h.maxHp * _baseHpFactor).CeilingInt();
                Log.Info($"Rebalancing - {h.name} - MaxHP - {h.maxHp}");
                EditorUtility.SetDirty(h);
            });
            
            Log.Info("Rebalance Heroes Finished");
        }
        
        _growthHpFactor = EditorGUILayout.FloatField("Growth HP Factor", _growthHpFactor);
        if (GUILayout.Button("Rebalance HP Growth")) 
        {
            Log.Info("Rebalance Heroes Begun");
            var heroes =  UnityResourceUtils.FindAssetsByType<BaseHero>()
                .Where(h => !h.name.Equals("Anon"))
                .Where(h => h.LevelUpTreeV4 != null)
                .ToArray();
            
            heroes.ForEach(h =>
            {
                var originalHpGrowth = h.LevelUpTreeV4.TotalHpGrowth;
                var targetHpGrowth = (originalHpGrowth * _growthHpFactor).CeilingInt();
                var numberOfLevelUps = h.LevelUpTreeV4.EditorRewards.Length;
                var hpDistro = Enumerable.Range(0, numberOfLevelUps).Select(x => (targetHpGrowth / (float)numberOfLevelUps).FlooredInt()).ToArray();
                var remainder = targetHpGrowth % numberOfLevelUps;
                var index = numberOfLevelUps - 1;
                while (remainder > 0)
                {
                    hpDistro[index]++;
                    remainder--;
                    index = (index - 2) % hpDistro.Length;
                }

                var levelUps = h.LevelUpTreeV4.EditorRewards;
                for(var i = 0; i < hpDistro.Length; i++)
                {
                    var levelUp = levelUps[i];
                    levelUp.EditorSetHpGain(hpDistro[i]);
                }
                Log.Info($"Rebalancing - {h.name} - Growth HP - {targetHpGrowth}");
                EditorUtility.SetDirty(h.LevelUpTreeV4);
            });
            
            Log.Info("Rebalance Heroes Finished");
        }

        if (GUILayout.Button("Show Hero HP"))
        {
            var heroes = UnityResourceUtils.FindAssetsByType<BaseHero>()
                .Where(h => !h.name.Equals("Anon"))
                .Where(h => h.LevelUpTreeV4 != null)
                .ToArray();
            var heroDetails = heroes.Select(h =>
                $"{h.name.PadRight(10, ' ')} - Base HP: {h.maxHp} - Total Growth: {HpGrowth(h)} - Final HP: {h.maxHp + HpGrowth(h)}").ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized("Hero HP Summary", heroDetails)
                .Show();
        }
    }

    private static int HpGrowth(BaseHero h) =>
        h.LevelUpTreeV4 != null ? h.LevelUpTreeV4.TotalHpGrowth : 0;
}
#endif
