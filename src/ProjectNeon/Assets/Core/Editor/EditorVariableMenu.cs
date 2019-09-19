using UnityEngine;
using UnityEditor;

static class EditorVariableMenu
{
    [MenuItem("Assets/Create/Variable/Int Variable")]
    static void IntVariable()
    {
        var asset = ScriptableObject.CreateInstance<IntVariable>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{nameof(IntVariable)}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }

    [MenuItem("Assets/Create/Variable/Float Variable")]
    static void FloatVariable()
    {
        var asset = ScriptableObject.CreateInstance<FloatVariable>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{nameof(FloatVariable)}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }

    [MenuItem("Assets/Create/Variable/String Variable")]
    static void StringVariable()
    {
        var asset = ScriptableObject.CreateInstance<StringVariable>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{nameof(StringVariable)}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}