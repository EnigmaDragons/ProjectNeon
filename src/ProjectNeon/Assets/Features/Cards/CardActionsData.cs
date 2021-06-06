using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect")]
public class CardActionsData : ScriptableObject
{
    public CardActionV2[] Actions = new CardActionV2[0];

    public IEnumerable<EffectData> BattleEffects => Actions
        .Where(x => x.Type == CardBattleActionType.Battle)
        .Select(a => a.BattleEffect);
    
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
