using System.Collections.Generic;
using System.Linq;
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
}
