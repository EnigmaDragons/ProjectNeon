#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ApplyPrefabToAllCombatants : EditorWindow
{
    private static T[] GetAllInstances<T>() where T : ScriptableObject 
        => AssetDatabase.FindAssets("t:" + typeof(T).Name)
            .Select(x => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(x))).ToArray();
    
    [MenuItem("Neon/Prefabs/Apply Effect to All Prefabs %#_E")]
    public static void ApplyBlinded()
    {
        ApplyPrefabToHeroes<BlindedEffect>("t:prefab VFX_Blinded_Persistent_Hero");
        //ApplyPrefabToEnemies<BlindedEffect>("t:prefab VFX_Blinded_Persistent");
    }

    private static void ApplyPrefabToHeroes<TUniqueComponent>(string searchString)
    {
        Log.Info($"Starting Applying {searchString} to all Heroes");
        var effects = AssetDatabase.FindAssets(searchString);
        if (effects.Length != 1)
        {
            Log.Error("Count not find Effect");
            return;
        }

        var effectGuid = effects[0];
        var effect = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(effectGuid));
        var heroes = GetAllInstances<BaseHero>();
        foreach (var h in heroes)
            if (h.Body != null)
                ApplyPrefabAtCenterpoint<TUniqueComponent>(h.name, h.Body, searchString, effect);

        AssetDatabase.SaveAssets();
        Log.Info("Finished");
    }

    private static void ApplyPrefabToEnemies<TUniqueComponent>(string searchString)
    {
        Log.Info($"Starting Applying {searchString} to all Enemies");
        var effects = AssetDatabase.FindAssets(searchString);
        if (effects.Length != 1)
        {
            Log.Error("Count not find Effect");
            return;
        }

        var effectGuid = effects[0];
        var effect = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(effectGuid));
        var enemies = GetAllInstances<Enemy>();
        foreach (var e in enemies)
            if (e.IsCurrentlyWorking && e.Prefab != null)
                ApplyPrefabAtCenterpoint<TUniqueComponent>(e.name, e.Prefab, searchString, effect);
        
        AssetDatabase.SaveAssets();
        Log.Info("Finished");
    }
    
    private static void ApplyPrefabAtCenterpoint<TUniqueComponent>(string oName, GameObject bodyPrefab, string effectTypeDesc, GameObject effect)
    {
        try
        {
            var prefab = (GameObject)PrefabUtility.InstantiatePrefab(bodyPrefab);
            prefab.hideFlags = HideFlags.HideInHierarchy;
            var matchingPrefabs = prefab.GetComponentsInChildren<TUniqueComponent>();
            if (matchingPrefabs.Length > 0)
            {
                Log.Info($"{oName} - Has Effect '{effectTypeDesc}'");
                DestroyImmediate(prefab);
                return;
            }

            Log.Info($"{oName} - Started Adding Effect '{effectTypeDesc}'");
            var effectInstance = (GameObject)PrefabUtility.InstantiatePrefab(effect);

            var centerPoint = GetCenterPointPosition(prefab.transform);
            effectInstance.transform.position = centerPoint;
            effectInstance.transform.SetParent(prefab.transform);

            var enemyPrefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
            PrefabUtility.SaveAsPrefabAssetAndConnect(prefab, enemyPrefabAssetPath,
                InteractionMode.AutomatedAction);

            Log.Info($"{oName} - Added Effect '{effectTypeDesc}");

            DestroyImmediate(prefab);
            DestroyImmediate(effectInstance);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            Log.Error($"Unable to add effect to {oName}");
        }
    }

    private static Vector3 GetCenterPointPosition(Transform t)
    {
        var centerPoint = t.GetComponentInChildren<CenterPoint>();
        if (centerPoint != null)
            return centerPoint.transform.position;

        return Vector3.zero;
    }
}

#endif
