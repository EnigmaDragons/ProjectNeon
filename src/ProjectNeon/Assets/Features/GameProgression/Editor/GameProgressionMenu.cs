
using UnityEditor;
using UnityEngine;

public static class GameProgressionMenu
{
    [MenuItem("Assets/Create/Adventure/Stage")]
    static void Stage() => Create<Stage>();
    [MenuItem("Assets/Create/Adventure/FixedEncounter")]
    static void FixedEncounter() => Create<SpecificEncounterSegment>();
    [MenuItem("Assets/Create/Adventure/Adventure")]
    static void Adventure() => Create<Adventure>();
    [MenuItem("Assets/Create/Adventure/ShopSegment")]
    static void ShoppingSegment() => Create<ShopSegment>();

    private static void Create<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{typeof(T).Name}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}
