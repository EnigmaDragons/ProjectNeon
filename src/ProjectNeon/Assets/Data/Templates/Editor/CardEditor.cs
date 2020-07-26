using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    private Card targetCard;
    private SerializedProperty art, description, typeDescription, onlyPlayableByClass, cost, actionSequences, cardAction1, cardAction2;

    public void OnEnable()
    {
        targetCard = (Card) target;
        art = serializedObject.FindProperty("art");
        description = serializedObject.FindProperty("description");
        typeDescription = serializedObject.FindProperty("typeDescription");
        onlyPlayableByClass = serializedObject.FindProperty("onlyPlayableByClass");
        cost = serializedObject.FindProperty("cost");
        actionSequences = serializedObject.FindProperty("actionSequences");
        cardAction1 = serializedObject.FindProperty("cardAction1");
        cardAction2 = serializedObject.FindProperty("cardAction2");
    }

    public override void OnInspectorGUI()
    {
        PresentUnchanged(art);
        PresentUnchanged(description);
        PresentUnchanged(typeDescription);
        PresentUnchanged(onlyPlayableByClass);
        PresentUnchanged(cost);
        PresentActionSequences();
        //PresentUnchanged(actionSequences);
        PresentUnchanged(cardAction1);
        PresentUnchanged(cardAction2);
    }

    private void PresentUnchanged(SerializedProperty serializedProperty)
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
        serializedObject.ApplyModifiedProperties();
    }

    private void PresentActionSequences()
    {
        EditorGUI.indentLevel = 0;
        PresentLabelsWithControls("Action Sequences", menu => menu.AddItem(new GUIContent("Insert New"), false, () =>
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
            PresentLabelsWithControls($"Action Sequence {refBrokenI}", menu =>
            {
                menu.AddItem(new GUIContent("Add"), false, () =>
                {
                    targetCard.actionSequences = sequences.Take(Array.IndexOf(sequences, sequence) + 1)
                        .Concat(new CardActionSequence[] {new CardActionSequence()})
                        .Concat(sequences.Skip(Array.IndexOf(sequences, sequence) + 1))
                        .ToArray();
                    EditorUtility.SetDirty(target);
                });
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    targetCard.actionSequences = sequences.Where(x => x != sequence).ToArray();
                    EditorUtility.SetDirty(target);
                });
            });
            EditorGUI.indentLevel++;
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].group"));
            PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].scope"));
            PresentLabelsWithControls("Actions", menu => menu.AddItem(new GUIContent("Insert New"), false, () =>
            {
                sequence.cardActions = new CardActionV2[] { new CardActionV2() }.Concat(sequence.cardActions).ToArray();
                EditorUtility.SetDirty(target);
            }));
            EditorGUI.indentLevel++;
            var actions = sequence.cardActions.ToArray();
            for (var ii = 0; ii < actions.Length; ii++)
            {
                var refBrokenii = ii;
                var action = actions[refBrokenii];
                PresentLabelsWithControls($"Action {refBrokenii}", menu =>
                {
                    menu.AddItem(new GUIContent("Add"), false, () =>
                    {
                        sequence.cardActions = sequence.cardActions.Take(Array.IndexOf(sequence.cardActions, action) + 1)
                            .Concat(new CardActionV2[] {new CardActionV2()})
                            .Concat(sequence.cardActions.Skip(Array.IndexOf(sequence.cardActions, action) + 1))
                            .ToArray();
                        EditorUtility.SetDirty(target);
                    });
                    menu.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        sequence.cardActions = sequence.cardActions.Where(x => x != action).ToArray();
                        EditorUtility.SetDirty(target);
                    });
                });
                EditorGUI.indentLevel++;
                PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].cardActions.Array.data[{refBrokenii}].type"));
                if (action.Type == CardBattleActionType.AnimateCharacter)
                    PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].cardActions.Array.data[{refBrokenii}].characterAnimation"));
                if (action.Type == CardBattleActionType.AnimateAtTarget)
                    PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].cardActions.Array.data[{refBrokenii}].atTargetAnimation"));
                if (action.Type == CardBattleActionType.Battle)
                    PresentUnchanged(serializedObject.FindProperty($"actionSequences.Array.data[{refBrokenI}].cardActions.Array.data[{refBrokenii}].battleEffect"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
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
}