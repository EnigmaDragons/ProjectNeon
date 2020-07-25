using UnityEditor;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    private SerializedProperty art, description, typeDescription, onlyPlayableByClass, cost, actionSequences, cardAction1, cardAction2;

    public void OnEnable()
    {
        art = serializedObject.FindProperty("art");
        description = serializedObject.FindProperty("description");
        typeDescription = serializedObject.FindProperty("typeDescription");
        onlyPlayableByClass = serializedObject.FindProperty("onlyPlayableByClass");
        cost = serializedObject.FindProperty("cost");
        actionSequences = serializedObject.FindProperty("actionSequences");
        cardAction1 = serializedObject.FindProperty("cardAction1");
        cardAction2 = serializedObject.FindProperty("cardAction2");
    }

    public override void OnInspectorGUI()
    {
        DrawUnchanged(art);
        DrawUnchanged(description);
        DrawUnchanged(typeDescription);
        DrawUnchanged(onlyPlayableByClass);
        DrawUnchanged(cost);
        DrawUnchanged(actionSequences);
        DrawUnchanged(cardAction1);
        DrawUnchanged(cardAction2);
    }

    private void DrawUnchanged(SerializedProperty serializedProperty)
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedProperty);
        serializedObject.ApplyModifiedProperties();
    }
}