using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EffectData))]
public class EffectDataEditor : PropertyDrawer
{
    private DictionaryWithDefault<EffectType, string[]> _relevantProperties = new DictionaryWithDefault<EffectType, string[]>(new string[] { "FloatAmount", "NumberOfTurns", "EffectScope" })
    {
        
    };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => (_relevantProperties[GetEffectType(property)].Length + 1) * 18 - 2;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = 16;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("EffectType"));
        var properties = _relevantProperties[GetEffectType(property)];
        for (var i = 0; i < properties.Length; i++)
        {
            position.y += 18;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(properties[i]));
        }
    }

    private EffectType GetEffectType(SerializedProperty property)
    {
        var effectType = property.FindPropertyRelative("EffectType");
        return (EffectType)Enum.GetValues(typeof(EffectType)).GetValue(effectType.enumValueIndex);
    }
}