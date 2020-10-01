﻿#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EffectData))]
public class EffectDataEditor : PropertyDrawer
{
    private List<string> _globalProperties = new List<string> { "AtStartOfNextTurn" };
    
    private DictionaryWithDefault<EffectType, string[]> _relevantProperties = new DictionaryWithDefault<EffectType, string[]>(new string[] { "FloatAmount", "NumberOfTurns", "HitsRandomTargetMember", "BaseAmount" })
    {
        {EffectType.Nothing, new string[0]},
        {EffectType.OnAttacked, new []{ "FloatAmount", "NumberOfTurns", "EffectScope", "StatusTag", "ReactionSequence" }},
        {EffectType.OnEvaded, new []{ "FloatAmount", "NumberOfTurns", "EffectScope", "ReactionSequence" }},
        {EffectType.OnShieldBroken, new []{ "NumberOfTurns", "EffectScope", "ReactionSequence" }},
        {EffectType.OnDamaged, new []{ "FloatAmount", "NumberOfTurns", "EffectScope", "ReactionSequence" }},
        {EffectType.CostResource, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.ResourceFlat, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.RemoveDebuffs, new [] { "NumberOfTurns" }},
        {EffectType.AdjustStatAdditively, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.AdjustStatAdditivelyWithMagic, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.AdjustStatMultiplicatively, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.AdjustTemporaryStatAdditively, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.AdjustStatAdditivelyBaseOnMagicStat, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.AdjustPlayerStats, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.EvadeAttacks, new [] { "FloatAmount" }},
        {EffectType.AtStartOfTurn, new [] { "NumberOfTurns", "EffectScope", "ReferencedSequence" }},
        {EffectType.AtEndOfTurn, new [] { "NumberOfTurns", "EffectScope", "ReferencedSequence" }},
        {EffectType.DuplicateStatesOfType, new [] { "StatusTag" }},
    };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("EffectType"));
        foreach (var prop in _relevantProperties[GetEffectType(property)])
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(prop)) + 2;
        foreach (var prop in _globalProperties)
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

        for (var i = 0; i < _globalProperties.Count; i++)
        {
            var prop = property.FindPropertyRelative(_globalProperties[i]);
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

#endif
