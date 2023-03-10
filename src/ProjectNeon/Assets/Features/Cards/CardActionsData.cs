﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect", order = -99)]
public class CardActionsData : ScriptableObject, ILocalizeTerms
{
    private string _testObjectName;
    
    public string Name => _testObjectName != null ? _testObjectName : this != null ? name : "Unnamed" + nameof(CardActionsData);
    public CardActionV2[] Actions = new CardActionV2[0];

    public IEnumerable<EffectData> BattleEffects => (Actions ?? new CardActionV2[0])
        .Where(x => x.Type == CardBattleActionType.Battle && x.BattleEffect != null)
        .Select(a => a.BattleEffect);
    
    public IEnumerable<EffectData> InnerBattleEffects => (Actions ?? new CardActionV2[0])
        .Where(x => x.Type == CardBattleActionType.Battle && x.BattleEffect != null && x.BattleEffect.ReferencedSequence != null)
        .SelectMany(a => a.BattleEffect.ReferencedSequence.BattleEffects.Concat(a.BattleEffect.ReferencedSequence.InnerBattleEffects));
    
    public IEnumerable<EffectData> ConditionalBattleEffects => Actions
        .Where(x => x.Type == CardBattleActionType.Condition)
        .SelectMany(a => a.ConditionData.ReferencedEffect.BattleEffects);

    public int NumAnimations => Actions.Count(
        x => x.Type == CardBattleActionType.AnimateCharacter 
             || x.Type == CardBattleActionType.AnimateAtTarget);
    
    public CardActionsData Initialized(params CardActionV2[] actions)
    {
        _testObjectName = "TestCardActions";
        Actions = actions;
        return this;
    }
    
    public CardActionsData CloneForBuyout(EffectData buyoutData) => ((CardActionsData)FormatterServices.GetUninitializedObject(typeof(CardActionsData)))
        .Initialized(Actions.Select(x => x.Type == CardBattleActionType.Battle && x.BattleEffect.EffectType == EffectType.BuyoutEnemyById 
            ? x.Clone(buyoutData) 
            : x).ToArray());

    public string[] GetLocalizeTerms()
        => Actions
            .Where(x => x.Type == CardBattleActionType.Battle && !string.IsNullOrWhiteSpace(x.BattleEffect.StatusDetailText))
            .Select(x => x.BattleEffect.StatusDetailTerm)
            .Concat(Actions
                .Where(x => x.Type == CardBattleActionType.Battle && (x.BattleEffect.EffectType == EffectType.ApplyAdditiveStatInjury || x.BattleEffect.EffectType == EffectType.ApplyMultiplicativeStatInjury))
                .Select(x => $"Injuries/{x.BattleEffect.FlavorText}"))
            .Concat(Actions.SelectMany(x =>
            {
                var results = new List<string>();
                if (x.Type != CardBattleActionType.Battle)
                    return results;
                if (!string.IsNullOrWhiteSpace(x.BattleEffect.InterpolatePartialFormula.PrefixTerm))
                    results.Add($"CardInterpolations/Prefix-{x.BattleEffect.InterpolatePartialFormula.PrefixTerm}");
                if (!string.IsNullOrWhiteSpace(x.BattleEffect.InterpolatePartialFormula.SuffixTerm))
                    results.Add($"CardInterpolations/Suffix-{x.BattleEffect.InterpolatePartialFormula.SuffixTerm}");
                return results;
            }))
            .ToArray();
}
