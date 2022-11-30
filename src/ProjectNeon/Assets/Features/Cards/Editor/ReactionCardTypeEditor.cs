#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ReactionCardType))]
public class ReactionCardTypeEditor : Editor
{
    private ReactionCardType targetCard;
    private SerializedProperty id, displayName, art, description, descriptionV2, archetypes, cost, actionSequence, tags;

    public void OnEnable()
    {
        targetCard = (ReactionCardType) target;
        id = serializedObject.FindProperty("id");
        displayName = serializedObject.FindProperty("displayName");
        art = serializedObject.FindProperty("art");
        description = serializedObject.FindProperty("description");
        descriptionV2 = serializedObject.FindProperty("descriptionV2");
        archetypes = serializedObject.FindProperty("archetypes");
        cost = serializedObject.FindProperty("cost");
        actionSequence = serializedObject.FindProperty("actionSequence");
        tags = serializedObject.FindProperty("tags");
    }
    
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        PresentUnchanged(id);
        GUI.enabled = true;
        PresentUnchanged(displayName);
        PresentUnchanged(art);
        
        var descV2 = GetSerializedValue<CardDescriptionV2>(descriptionV2);
        
        if (descV2.IsUsable())
        {
            if (GUILayout.Button("Copy V2 I2 English Description"))
            {
                GUIUtility.systemCopyBuffer = targetCard.descriptionV2.ToSingleLineI2Format();
            }
            GUILayout.Label("Description V2 - Preview:", new GUIStyle() { fontStyle = FontStyle.Bold });
            GUILayout.TextArea(descV2.Preview(), new GUIStyle { padding = new RectOffset(0,0,8,8), stretchHeight = true});
            GUILayout.Label("Description V2 - Library Sample:", new GUIStyle() { fontStyle = FontStyle.Bold });
            GUILayout.TextArea(targetCard.EditorOnlyLibraryPreview().Replace("<br>", "\r\n"), 
                new GUIStyle { padding = new RectOffset(0,0,8,8), stretchHeight = true});
        }
        else
        {        
            if (GUILayout.Button("Generate Auto Description V1"))
            {
                targetCard.description = targetCard.AutoDescription(Maybe<Member>.Missing(), ResourceQuantity.None);
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("Generate Auto Partial V1"))
            {
                targetCard.description = targetCard.AutoPartial();
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("Convert To V2 Description"))
            {
                targetCard.descriptionV2 = CardDescriptionV2.FromDescriptionV1(GetSerializedValue<string>(description));
                EditorUtility.SetDirty(target);
            }
            PresentUnchanged(description, "Description V1");
        }
        
        PresentUnchanged(descriptionV2);
        PresentUnchanged(archetypes);
        PresentUnchanged(cost);
        PresentUnchanged(actionSequence);
        PresentUnchanged(tags);
    }
    
    private void PresentUnchanged(SerializedProperty serializedProperty)
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
        serializedObject.ApplyModifiedProperties();
    }
    
    private void PresentUnchanged(SerializedProperty serializedProperty, string label)
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label), includeChildren: true);
        serializedObject.ApplyModifiedProperties();
    }
    
    public static T GetSerializedValue<T>(SerializedProperty property)
    {
        object @object = property.serializedObject.targetObject;
        string[] propertyNames = property.propertyPath.Split('.');
 
        // Clear the property path from "Array" and "data[i]".
        if (propertyNames.Length >= 3 && propertyNames[propertyNames.Length - 2] == "Array")
            propertyNames = propertyNames.Take(propertyNames.Length - 2).ToArray();
 
        // Get the last object of the property path.
        foreach (string path in propertyNames)
        {
            @object = @object.GetType()
                .GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .GetValue(@object);
        }
 
        if (@object.GetType().GetInterfaces().Contains(typeof(IList<T>)))
        {
            int propertyIndex = int.Parse(property.propertyPath[property.propertyPath.Length - 2].ToString());
 
            return ((IList<T>) @object)[propertyIndex];
        }
        else return (T) @object;
    }
}

#endif