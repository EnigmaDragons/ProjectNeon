#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DirectionalInputNode))]
public class DirectionalInputNodeEditor : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 50;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = 16;
        position.width = 200;
        position.y += 1;
        position.x += 200;
        EditorGUI.PropertyField(position, property.FindPropertyRelative($"Up"), new GUIContent());
        position.x -= 200;
        position.y += 16;
        EditorGUI.PropertyField(position, property.FindPropertyRelative($"Left"), new GUIContent());
        position.x += 200;
        EditorGUI.PropertyField(position, property.FindPropertyRelative($"Selectable"), new GUIContent());
        position.x += 200;
        EditorGUI.PropertyField(position, property.FindPropertyRelative($"Right"), new GUIContent());
        position.x -= 200;
        position.y += 16;
        EditorGUI.PropertyField(position, property.FindPropertyRelative($"Down"), new GUIContent());
    }
}
#endif