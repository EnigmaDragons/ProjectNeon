using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EffectData))]
public class EffectDataEditor : PropertyDrawer
{
    private DictionaryWithDefault<EffectType, string[]> _relevantProperties = new DictionaryWithDefault<EffectType, string[]>(new string[] { "FloatAmount", "NumberOfTurns" })
    {
        {EffectType.Nothing, new string[0]},
        {EffectType.OnAttacked, new []{ "NumberOfTurns", "ReferencedEffectSequence" }},
        {EffectType.CostResource, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.ResourceFlat, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }}
    };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("EffectType"));
        foreach (var prop in _relevantProperties[GetEffectType(property)])
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(prop)) + 2;
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = 16;
        var effectType = property.FindPropertyRelative("EffectType");
        EditorGUI.PropertyField(position, effectType);
        position.y += EditorGUI.GetPropertyHeight(effectType) + 2;
        var properties = _relevantProperties[GetEffectType(property)];
        for (var i = 0; i < properties.Length; i++)
        {
            var prop = property.FindPropertyRelative(properties[i]);
            EditorGUI.PropertyField(position, prop);
            position.y += EditorGUI.GetPropertyHeight(effectType) + 2;
        }
    }

    private EffectType GetEffectType(SerializedProperty property)
    {
        var effectType = property.FindPropertyRelative("EffectType");
        return (EffectType)Enum.GetValues(typeof(EffectType)).GetValue(effectType.enumValueIndex);
    }
}