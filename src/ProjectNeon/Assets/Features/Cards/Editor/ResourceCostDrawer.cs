#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResourceCost))]
public class ResourceCostDrawer : PropertyDrawer
{
    private SerializedProperty _cost;
    private SerializedProperty _resourceType;
    private SerializedProperty _plusXCost;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 34;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var cost = _cost ?? (_cost = property.FindPropertyRelative("cost"));
        var resourceType = _resourceType ?? (_resourceType = property.FindPropertyRelative("resourceType"));
        var plusXCost = _plusXCost ?? (_plusXCost = property.FindPropertyRelative("plusXCost"));
        position.height = 16f;
        Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
        EditorGUI.BeginProperty(contentPosition, label, cost);
        {
            EditorGUI.BeginChangeCheck();
            bool newVal = EditorGUI.Toggle(contentPosition, new GUIContent("Plus X"), plusXCost.boolValue);
            if (EditorGUI.EndChangeCheck())
                plusXCost.boolValue = newVal;
        }

        EditorGUI.indentLevel += 1;
        contentPosition = EditorGUI.IndentedRect(position);
        contentPosition.y += 18f;

        float quarter = contentPosition.width / 4;
        contentPosition.width = quarter;
        EditorGUI.BeginProperty(contentPosition, label, cost);
        {
            EditorGUI.BeginChangeCheck();
            int newVal = EditorGUI.IntField(contentPosition, new GUIContent(), cost.intValue);
            if (EditorGUI.EndChangeCheck())
                cost.intValue = newVal;
        }
        EditorGUI.EndProperty();
        contentPosition.x += quarter;
        contentPosition.width = quarter * 3;
        
        EditorGUI.BeginProperty(contentPosition, label, resourceType);
        {
            EditorGUI.BeginChangeCheck();
            Object newVal = EditorGUI.ObjectField(contentPosition, resourceType.objectReferenceValue, typeof(ResourceType), false);
            if (EditorGUI.EndChangeCheck())
                resourceType.objectReferenceValue = newVal;
        }
        EditorGUI.EndProperty();
        EditorGUI.indentLevel -= 1;
    }
}

#endif
