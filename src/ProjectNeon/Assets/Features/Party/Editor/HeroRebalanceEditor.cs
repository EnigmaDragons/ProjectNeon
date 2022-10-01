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

    private float _hpFactor = 1f;
    
    void OnGUI()
    {
        _hpFactor = EditorGUILayout.FloatField("HP Factor", _hpFactor);
        if (GUILayout.Button("Rebalance")) 
        {
            Log.Info("Rebalance Heroes Begun");
            var heroes =  UnityResourceUtils.FindAssetsByType<BaseHero>()
                .Where(h => !h.Name.Equals("Anon"))
                .ToArray();
            
            heroes.ForEach(h =>
            {
                h.maxHp = (h.maxHp * _hpFactor).CeilingInt();
                Log.Info($"Rebalancing - {h.name} - MaxHP - {h.maxHp}");
                EditorUtility.SetDirty(h);
            });
            
            Log.Info("Rebalance Heroes Finished");
        }
    }
}
#endif
