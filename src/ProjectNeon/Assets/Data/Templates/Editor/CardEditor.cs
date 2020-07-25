using UnityEditor;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    private SerializedProperty _art;
    private SerializedProperty _description;
    private SerializedProperty _typeDescription;
    private SerializedProperty _onlyPlayableByClass;
    private SerializedProperty _cost;
    private SerializedProperty _classSpecial;


    public void OnEnable()
    {
        _art = serializedObject.FindProperty("art");
        _description = serializedObject.FindProperty("description");
        _typeDescription = serializedObject.FindProperty("typeDescription");
        _onlyPlayableByClass = serializedObject.FindProperty("onlyPlayableByClass");
        _cost = serializedObject.FindProperty("cost");
        _classSpecial = serializedObject.FindProperty("classSpecial");
    }

    /*public override void OnInspectorGUI()
    {
        EditorGUILayout.ObjectField(_art);
        AssetPreview.GetAssetPreview(_art.objectReferenceValue);
        EditorGUILayout.ObjectField(_description);
        EditorGUILayout.ObjectField(_typeDescription);
        EditorGUILayout.ObjectField(_onlyPlayableByClass);
        EditorGUILayout.ObjectField(_cost);
        EditorGUILayout.ObjectField(_classSpecial);
    }*/
}