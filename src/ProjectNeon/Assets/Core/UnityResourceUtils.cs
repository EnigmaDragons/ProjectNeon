#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public sealed class UnityResourceUtils
{
    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        return AssetDatabase.FindAssets($"t:{typeof(T)}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>)
            .Where(asset => asset != null)
            .ToList();
    }
}
#endif
