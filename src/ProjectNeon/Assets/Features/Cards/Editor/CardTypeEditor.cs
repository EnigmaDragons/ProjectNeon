#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardType))]
public class CardTypeEditor : Editor
{
    private CardType targetCard;
    private SerializedProperty customName, functionalityIssues, art, description, descriptionV2, typeDescription, tags, 
        cost, rarity, cardAction1, cardAction2, chainedCard, presentationIssues, speed, archetypes, isWip, 
        highlightCondition, unhighlightCondition, targetedHighlightCondition, targetedUnhighlightCondition, 
        swappedCard, isSinglePlay, id, notAvailableForGeneralDistribution;

    public void OnEnable()
    {
        targetCard = (CardType) target;
        customName = serializedObject.FindProperty("customName");
        art = serializedObject.FindProperty("art");
        description = serializedObject.FindProperty("description");
        descriptionV2 = serializedObject.FindProperty("descriptionV2");
        typeDescription = serializedObject.FindProperty("typeDescription");
        tags = serializedObject.FindProperty("tags");
        cost = serializedObject.FindProperty("cost");
        rarity = serializedObject.FindProperty("rarity");
        chainedCard = serializedObject.FindProperty("chainedCard");
        swappedCard = serializedObject.FindProperty("swappedCard");
        functionalityIssues = serializedObject.FindProperty("functionalityIssues");
        presentationIssues = serializedObject.FindProperty("presentationIssues");
        speed = serializedObject.FindProperty("speed");
        archetypes = serializedObject.FindProperty("archetypes");
        isWip = serializedObject.FindProperty("isWIP");
        notAvailableForGeneralDistribution = serializedObject.FindProperty("notAvailableForGeneralDistribution");
        highlightCondition = serializedObject.FindProperty("highlightCondition");
        unhighlightCondition = serializedObject.FindProperty("unhighlightCondition");
        targetedHighlightCondition = serializedObject.FindProperty("targetedHighlightCondition");
        targetedUnhighlightCondition = serializedObject.FindProperty("targetedUnhighlightCondition");
        isSinglePlay = serializedObject.FindProperty("isSinglePlay");
        id = serializedObject.FindProperty("id");
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        PresentUnchanged(id);
        GUI.enabled = true;
        PresentUnchanged(customName);
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
        PresentUnchanged(typeDescription);
        PresentUnchanged(archetypes);
        PresentUnchanged(rarity);
        DrawUILine(Color.black);
        GUILayout.Label($"Is Dodgeable/Blind Preventable: {targetCard.IsAttack().ToString()}", new GUIStyle { fontStyle = FontStyle.Bold });
        GUILayout.Label($"Is Inhibitable: {(!targetCard.IsAttack()).ToString()}", new GUIStyle { fontStyle = FontStyle.Bold });
        PresentUnchanged(speed);
        PresentUnchanged(isSinglePlay);
        PresentUnchanged(cost);
        PresentActionSequences();
        PresentUnchanged(chainedCard);
        PresentUnchanged(swappedCard);
        DrawUILine(Color.black);
        PresentUnchanged(highlightCondition);
        PresentUnchanged(unhighlightCondition);
        PresentUnchanged(targetedHighlightCondition);
        PresentUnchanged(targetedUnhighlightCondition);
        DrawUILine(Color.black);
        PresentUnchanged(tags);
        DrawUILine(Color.black);
        PresentUnchanged(isWip);
        PresentUnchanged(notAvailableForGeneralDistribution);
        PresentUnchanged(functionalityIssues);
        PresentUnchanged(presentationIssues);
        DrawUILine(Color.black);
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

    private void PresentActionSequences()
    {
        EditorGUI.indentLevel = 0;
        PresentLabelsWithControls("Command Sequence", menu => menu.AddItem(new GUIContent("Insert New"), false, () =>
        {
            targetCard.actionSequences = new CardActionSequence[] {new CardActionSequence()}.Concat(targetCard.actionSequences).ToArray();
            EditorUtility.SetDirty(target);
        }));
        EditorGUI.indentLevel++;
        var sequences = targetCard.actionSequences.ToArray();
        for (var i = 0; i < sequences.Length; i++)
        {
            var refBrokenI = i;
            var sequence = sequences[refBrokenI];
            PresentLabelsWithControls($"Command {refBrokenI}", menu =>
            {
                menu.AddItem(new GUIContent("Insert New After"), false, () =>
                {
                    targetCard.actionSequences = sequences.Take(Array.IndexOf(sequences, sequence) + 1)
                        .Concat(new CardActionSequence[] {new CardActionSequence()})
                        .Concat(sequences.Skip(Array.IndexOf(sequences, sequence) + 1))
                        .ToArray();
                    EditorUtility.SetDirty(target);
                });
                if (refBrokenI > 0)
                {
                    menu.AddItem(new GUIContent("Move Up"), false, () => targetCard.actionSequences.SwapItems(refBrokenI, refBrokenI - 1));
                    EditorUtility.SetDirty(target);
                }
                if (refBrokenI < sequences.Length - 1)
                {
                    menu.AddItem(new GUIContent("Move Down"), false, () => targetCard.actionSequences.SwapItems(refBrokenI, refBrokenI + 1));
                    EditorUtility.SetDirty(target);
                }
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    targetCard.actionSequences = sequences.Where(x => x != sequence).ToArray();
                    EditorUtility.SetDirty(target);
                });
            });
            EditorGUI.indentLevel++;
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].group"));
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].scope"));
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].repeatX"));
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].repeatCount"));
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].cardActions"), "Effect Sequence");
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
    }
    
    private void PresentLabelsWithControls(string label, Action<GenericMenu> addToGenericMenu)
    {
        EditorGUILayout.LabelField(label);
        var clickArea =  GUILayoutUtility.GetLastRect();
        var current = Event.current;
        if (clickArea.Contains(current.mousePosition) && current.type == EventType.ContextClick)
        {
            GenericMenu menu = new GenericMenu();
            addToGenericMenu(menu);
            menu.ShowAsContext();
            current.Use(); 
        }
    }
    
    private void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
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
