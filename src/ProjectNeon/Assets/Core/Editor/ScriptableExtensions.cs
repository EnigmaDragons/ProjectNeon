using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ScriptableExtensions
{
    public static ScriptableObject[] GetAll()
        => AssetDatabase.FindAssets("t:ScriptableObject")
            .Select(x => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(x))).ToArray();
    
    public static T[] GetAllInstances<T>() where T : ScriptableObject 
        => AssetDatabase.FindAssets("t:" + typeof(T).Name)
            .Select(x => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(x))).ToArray();
}