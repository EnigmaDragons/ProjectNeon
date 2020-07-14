#if UNITY_EDITOR
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

    [MenuItem("Assets/Create/Variable/Bool Variable")]
    static void BoolVariable()
    {
        var asset = ScriptableObject.CreateInstance<BoolVariable>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{nameof(BoolVariable)}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
    
    [MenuItem("Assets/Create/Variable/Vector3 Variable")]
    static void Vector3Variable()
    {
        var asset = ScriptableObject.CreateInstance<Vector3Variable>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{nameof(Vector3Variable)}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}
#endif
