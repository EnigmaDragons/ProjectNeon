#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ActionConditionData))]
public class ActionConditionDataDrawer : PropertyDrawer
{
    private DictionaryWithDefault<ActionConditionType, string[]> _relevantProperties = new DictionaryWithDefault<ActionConditionType, string[]>(new string[] { "FloatAmount" })
    {
        {ActionConditionType.Nothing, new string[0]},
        {ActionConditionType.PerformerHasResource, new [] { "FloatAmount", "EffectScope" }},
        {ActionConditionType.RepeatForSpent, new string[0]},
        {ActionConditionType.TargetSufferedDamage, new string[0]},
        {ActionConditionType.AllyIsUnconscious, new string[0]},
    };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("ConditionType"));
        foreach (var prop in _relevantProperties[GetConditionType(property)])
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(prop)) + 2;
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("ReferencedEffect")) + 2;
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = 16;
        var type = property.FindPropertyRelative("ConditionType");
        EditorGUI.PropertyField(position, type);
        position.y += EditorGUI.GetPropertyHeight(type) + 2;
        var properties = _relevantProperties[GetConditionType(property)];
        for (var i = 0; i < properties.Length; i++)
        {
            var prop = property.FindPropertyRelative(properties[i]);
            EditorGUI.PropertyField(position, prop);
            position.y += EditorGUI.GetPropertyHeight(prop) + 2;
        }
        EditorGUI.PropertyField(position, property.FindPropertyRelative("ReferencedEffect"));
    }

    private ActionConditionType GetConditionType(SerializedProperty property)
    {
        var effectType = property.FindPropertyRelative("ConditionType");
        return (ActionConditionType)Enum.GetValues(typeof(EffectType)).GetValue(effectType.enumValueIndex);
    }
}

#endif
