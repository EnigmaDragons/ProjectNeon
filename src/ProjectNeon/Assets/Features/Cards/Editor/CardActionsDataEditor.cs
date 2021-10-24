#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardActionsData))]
public class CardActionsDataEditor : Editor
{
    private CardActionsData _target;

    public void OnEnable()
    {
        _target = (CardActionsData) target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        var actions = _target.Actions.ToArray();
        PresentLabelsWithControls("Effect Sequence", menu => menu.AddItem(new GUIContent("Insert New First"), false, () =>
        {
            _target.Actions = new CardActionV2[] { new CardActionV2() }.Concat(actions).ToArray();
            EditorUtility.SetDirty(target);
        }));
        EditorGUI.indentLevel++;
        for (var i = 0; i < actions.Length; i++)
        {
            var refBrokeni = i;
            var action = actions[refBrokeni];
            PresentLabelsWithControls($"Effect {refBrokeni}", menu =>
            {
                menu.AddItem(new GUIContent("Insert New Before"), false, () =>
                {
                    _target.Actions = actions.Take(Array.IndexOf(actions, action))
                        .Concat(new CardActionV2[] {new CardActionV2()})
                        .Concat(actions.Skip(Array.IndexOf(actions, action)))
                        .ToArray();
                    EditorUtility.SetDirty(target);
                });
                menu.AddItem(new GUIContent("Insert New After"), false, () =>
                {
                    _target.Actions = actions.Take(Array.IndexOf(actions, action) + 1)
                        .Concat(new CardActionV2[] {new CardActionV2()})
                        .Concat(actions.Skip(Array.IndexOf(actions, action) + 1))
                        .ToArray();
                    EditorUtility.SetDirty(target);
                });
                if (refBrokeni > 0)
                    menu.AddItem(new GUIContent("Move Up"), false, () => _target.Actions.SwapItems(refBrokeni, refBrokeni - 1));
                if (refBrokeni < actions.Length - 1)
                    menu.AddItem(new GUIContent("Move Down"), false, () => _target.Actions.SwapItems(refBrokeni, refBrokeni + 1));
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    _target.Actions = actions.Where(x => x != action).ToArray();
                    EditorUtility.SetDirty(target);
                });
            });
            EditorGUI.indentLevel++;
            PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].type"));
            if (action.Type == CardBattleActionType.AnimateCharacter)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].characterAnimation2"));
            if (action.Type == CardBattleActionType.AnimateAtTarget)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].atTargetAnimation"));
            if (action.Type == CardBattleActionType.Battle)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].battleEffect"));
            if (action.Type == CardBattleActionType.Condition)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].conditionData"));
            
            if (action.Type == CardBattleActionType.SpawnEnemy)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].enemyToSpawn"));
            if (action.Type == CardBattleActionType.SpawnEnemy)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].enemySpawnOffset"));
            
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
    }
    
    private void PresentUnchanged(SerializedProperty serializedProperty)
    {
        serializedObject.Update();
        try
        {
            EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
        }
        catch (Exception e)
        {
            Log.Warn(e.Message);
        }
        serializedObject.ApplyModifiedProperties();
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

#endif
