using System;
using UnityEngine;

[Serializable]
public class CardActionV2
{
    [SerializeField] private CardBattleActionType type = CardBattleActionType.Battle;
    [SerializeField] private EffectData battleEffect = new EffectData();
    [SerializeField] private ActionConditionData conditionData = new ActionConditionData();
    [SerializeField] private AnimationData characterAnimation;
    [SerializeField] private StringReference atTargetAnimation;
    [SerializeField] private Enemy enemyToSpawn;

    public CardBattleActionType Type => type;
    public EffectData BattleEffect => battleEffect;
    public Enemy EnemyToSpawn => enemyToSpawn;
    public ActionConditionData ConditionData => conditionData;
    public AnimationData CharacterAnimation => characterAnimation;
    public StringReference AtTargetAnimation => atTargetAnimation;
    
    public CardActionV2() {}
    public CardActionV2(EffectData e)
    {
        battleEffect = e;
    }
}
