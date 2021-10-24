using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterAnimationStep))]
public class CharacterAnimationStepEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = 16;
        var stepTypeProp = property.FindPropertyRelative("StepType");
        EditorGUI.PropertyField(position, stepTypeProp);
        position.y += EditorGUI.GetPropertyHeight(stepTypeProp) + 2;
        var stepType = GetStepType(stepTypeProp);
        DisplayProperties(property, ref position, "Seconds");
        if (stepType == CharacterAnimationStepType.ChangeState)
            DisplayProperties(property, ref position, "Name", "Layer");
        if (stepType == CharacterAnimationStepType.Aim)
            DisplayProperties(property, ref position, "Aim");
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var stepTypeProp = property.FindPropertyRelative("StepType");
        var height = EditorGUI.GetPropertyHeight(stepTypeProp);
        var stepType = GetStepType(stepTypeProp);
        height += GetPropertyHeight(property, "Seconds");
        if (stepType == CharacterAnimationStepType.ChangeState)
            height += GetPropertiesHeight(property, "Name", "Layer");
        if (stepType == CharacterAnimationStepType.Aim)
            height += GetPropertiesHeight(property, "Aim");
        return height;
    }
    
    private CharacterAnimationStepType GetStepType(SerializedProperty property)
    {
        try
        {
            return (CharacterAnimationStepType) Enum.GetValues(typeof(CharacterAnimationStepType)).GetValue(property.enumValueIndex);
        }
        catch (Exception ex)
        {
            return CharacterAnimationStepType.PublishFinished;
        }
    }

    private void DisplayProperties(SerializedProperty property, ref Rect position, params string[] propNames)
    {
        foreach (var propName in propNames)
            DisplayProperty(property, ref position, propName);
    }

    private void DisplayProperty(SerializedProperty property, ref Rect position, string propName)
    {
        var prop = property.FindPropertyRelative(propName);
        var height = EditorGUI.GetPropertyHeight(prop);
        position.height = height;
        EditorGUI.PropertyField(position, prop);
        position.y += height + 2;
    }

    private float GetPropertiesHeight(SerializedProperty property, params string[] propNames)
    {
        var height = 0f;
        foreach (var propName in propNames)
            height += GetPropertyHeight(property, propName);
        return height;
    }
    
    private float GetPropertyHeight(SerializedProperty property, string propName)
        => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(propName), true) + 2;
}