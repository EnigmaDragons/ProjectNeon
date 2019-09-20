
using UnityEditor;
using UnityEngine;

public static class GameProgressionMenu
{
    [MenuItem("Assets/Create/GameContent/Stage")]
    static void Stage() => Create<Stage>();
    [MenuItem("Assets/Create/GameContent/FixedEncounter")]
    static void FixedEncounter() => Create<SpecificEncounterSegment>();

    private static void Create<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{typeof(T).Name}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}
