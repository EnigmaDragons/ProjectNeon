#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindBattleVFXEditor : EditorWindow
{
    [MenuItem("Neon/VFX Used")]
    static void EnemiesWanted()
    {
        GetWindow(typeof(FindBattleVFXEditor)).Show();
    }

    private VfxUsages[] vfxUsages;
    private Vector2 scrollPos;
    
    private void OnGUI()
    {
        if (GUILayout.Button("Show Unused VFX"))
        {
            var cards = GetAllInstances<CardType>();
            var effects = GetAllInstances<CardActionsData>().Where(cardActionsData 
                => cardActionsData.Actions != null && cardActionsData.Actions.Any(cardAction => cardAction.Type == CardBattleActionType.AnimateAtTarget)
                && cards.Any(card => card.ActionSequences != null && card.actionSequences.Any(cardActionSequence => cardActionSequence.CardActions?.Name == cardActionsData.Name)));
            var vfx = GetAllInstances<StringVariable>().Where(x 
                => x.name.StartsWith("BattleVFX-")
                && effects.None(cardActionsData => cardActionsData.Actions.Any(cardAction 
                    => cardAction.Type == CardBattleActionType.AnimateAtTarget 
                    && cardAction.AtTargetAnimation.Animation == x.Value)));
            vfxUsages = vfx.Select(x => new VfxUsages { Name = x.name, CardNames = new string[0] }).ToArray();
        }
        if (GUILayout.Button("Show Used VFX"))
        {
            var cards = GetAllInstances<CardType>();
            var vfx = GetAllInstances<StringVariable>().Where(x => x.name.StartsWith("BattleVFX-"));
            vfxUsages = vfx
                .Select(vfx => new VfxUsages
                {
                    Name = vfx.name, 
                    CardNames = cards
                        .Where(card => card.ActionSequences != null && card.ActionSequences
                            .Any(actionSequence => actionSequence.CardActions != null && actionSequence.CardActions.Actions
                                .Any(cardAction 
                                    => cardAction.Type == CardBattleActionType.AnimateAtTarget 
                                       && cardAction.AtTargetAnimation != null 
                                       && cardAction.AtTargetAnimation.Animation == vfx.Value)))
                        .Select(card => card.Name)
                        .ToArray()
                })
                .Where(x => x.CardNames.Length > 0)
                .ToArray();
        }

        if (vfxUsages == null)
            return;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var vfx in vfxUsages)
        {
            EditorGUILayout.LabelField($"{vfx.Name}");
            foreach (var cardName in vfx.CardNames)
                EditorGUILayout.LabelField($"    Card: {cardName}");
        }
        EditorGUILayout.EndScrollView();
    }

    private static T[] GetAllInstances<T>() where T : ScriptableObject 
        => AssetDatabase.FindAssets("t:" + typeof(T).Name)
            .Select(x => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(x))).ToArray();
    
    private void DrawUILine() => DrawUILine(Color.black);
    private void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
    }

    private class VfxUsages
    {
        public string Name { get; set; }
        public string[] CardNames { get; set; }
    }
}
#endif