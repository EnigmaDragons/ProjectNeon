using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect", order = -99)]
public class CardActionsData : ScriptableObject
{
    public CardActionV2[] Actions = new CardActionV2[0];

    public IEnumerable<EffectData> BattleEffects => (Actions ?? new CardActionV2[0])
        .Where(x => x.Type == CardBattleActionType.Battle && x.BattleEffect != null)
        .Select(a => a.BattleEffect);
    
    public IEnumerable<EffectData> ConditionalBattleEffects => Actions
        .Where(x => x.Type == CardBattleActionType.Condition)
        .SelectMany(a => a.ConditionData.ReferencedEffect.BattleEffects);

    public int NumAnimations => Actions.Count(
        x => x.Type == CardBattleActionType.AnimateCharacter 
             || x.Type == CardBattleActionType.AnimateAtTarget);
    
    public CardActionsData Initialized(params CardActionV2[] actions)
    {
        Actions = actions;
        return this;
    }
    
    public CardActionsData CloneForBuyout(EffectData buyoutData) => ((CardActionsData)FormatterServices.GetUninitializedObject(typeof(CardActionsData)))
        .Initialized(Actions.Select(x => x.Type == CardBattleActionType.Battle && x.BattleEffect.EffectType == EffectType.BuyoutEnemyById 
            ? x.Clone(buyoutData) 
            : x).ToArray());
}
