using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResourceCost))]
public class ResourceCostDrawer : PropertyDrawer
{
    private SerializedProperty _cost;
    private SerializedProperty _resourceType;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var cost = _cost ?? (_cost = property.FindPropertyRelative("cost"));
        var resourceType = _resourceType ?? (_resourceType = property.FindPropertyRelative("resourceType"));
        Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
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
    }
}