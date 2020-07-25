using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TransformLegacyCards
{
    [MenuItem("Enigma Dragons/Transform legacy Card Data")]
    public static void GoTransformLegacyCards()
    {
        /*GetAllInstances<Card>().ForEach(card => card.actionSequences = card.Actions.Select(cardAction =>
        {
            var cardActions = new List<CardActionV2>();
            if (!string.IsNullOrWhiteSpace(cardAction.CharacterAnimation))
                cardActions.Add(new CardActionV2 { type = CardBattleActionType.AnimateCharacter, characterAnimation = new StringReference(cardAction.CharacterAnimation) });
            if (!string.IsNullOrWhiteSpace(cardAction.EffectAnimation))
                cardActions.Add(new CardActionV2 { type = CardBattleActionType.AnimateAtTarget, atTargetAnimation = new StringReference(cardAction.EffectAnimation) });
            cardActions.AddRange(cardAction.Effects.Select(effect => new CardActionV2 { type = CardBattleActionType.Battle, battleEffect = effect }));
            return new CardActionSequence { scope = cardAction.Scope, @group = cardAction.Group, cardActions = cardActions.ToArray() };
        }).ToArray());
        AssetDatabase.Refresh();
        GetAllInstances<Card>().ForEach(EditorUtility.SetDirty);
        AssetDatabase.SaveAssets();*/
    }

    public static List<T> GetAllInstances<T>() where T : ScriptableObject
        => AssetDatabase.FindAssets("t:" + typeof(T).Name)
            .Select(x => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(x))).ToList();
}