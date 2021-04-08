#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardType))]
public class CardTypeEditor : Editor
{
    private CardType targetCard;
    private SerializedProperty customName, functionalityIssues, art, description, typeDescription, tags, onlyPlayableByClass, 
        cost, gain, rarity, actionSequences, cardAction1, cardAction2, chainedCard, presentationIssues, speed;

    public void OnEnable()
    {
        targetCard = (CardType) target;
        customName = serializedObject.FindProperty("customName");
        art = serializedObject.FindProperty("art");
        description = serializedObject.FindProperty("description");
        typeDescription = serializedObject.FindProperty("typeDescription");
        tags = serializedObject.FindProperty("tags");
        onlyPlayableByClass = serializedObject.FindProperty("onlyPlayableByClass");
        cost = serializedObject.FindProperty("cost");
        gain = serializedObject.FindProperty("onPlayGain");
        rarity = serializedObject.FindProperty("rarity");
        actionSequences = serializedObject.FindProperty("actionSequences");
        chainedCard = serializedObject.FindProperty("chainedCard");
        functionalityIssues = serializedObject.FindProperty("functionalityIssues");
        presentationIssues = serializedObject.FindProperty("presentationIssues");
        speed = serializedObject.FindProperty("speed");
    }

    public override void OnInspectorGUI()
    {
        PresentUnchanged(customName);
        PresentUnchanged(art);
        PresentUnchanged(description);
        PresentUnchanged(typeDescription);
        PresentUnchanged(onlyPlayableByClass);
        PresentUnchanged(rarity);
        DrawUILine(Color.black);
        PresentUnchanged(speed);
        PresentUnchanged(cost);
        PresentUnchanged(gain);
        PresentActionSequences();
        PresentUnchanged(chainedCard);
        DrawUILine(Color.black);
        PresentUnchanged(tags);
        DrawUILine(Color.black);
        PresentUnchanged(functionalityIssues);
        PresentUnchanged(presentationIssues);
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
                    menu.AddItem(new GUIContent("Move Up"), false, () => targetCard.actionSequences.SwapItems(refBrokenI, refBrokenI - 1));
                if (refBrokenI < sequences.Length - 1)
                    menu.AddItem(new GUIContent("Move Down"), false, () => targetCard.actionSequences.SwapItems(refBrokenI, refBrokenI + 1));
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    targetCard.actionSequences = sequences.Where(x => x != sequence).ToArray();
                    EditorUtility.SetDirty(target);
                });
            });
            EditorGUI.indentLevel++;
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].group"));
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].scope"));
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].avoidance"));
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].repeatX"));
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
}

#endif
