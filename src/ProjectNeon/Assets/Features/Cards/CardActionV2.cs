using System;
using UnityEngine;

[Serializable]
public class CardActionV2
{
    [SerializeField] private CardBattleActionType type = CardBattleActionType.Battle;
    [SerializeField] private EffectData battleEffect = new EffectData();
    [SerializeField] private ActionConditionData conditionData = new ActionConditionData();
    [SerializeField] private AnimationData characterAnimation;
    [SerializeField] private AnimationData2 characterAnimation2;
    [SerializeField] private AtTargetAnimationData atTargetAnimation;
    
    // TODO: Collapse these two together into an object with the previous. Create a tool to find all usages.
    [SerializeField] private Enemy enemyToSpawn;
    [SerializeField] private Vector3 enemySpawnOffset = Vector3.zero; 

    public CardBattleActionType Type => type;
    public EffectData BattleEffect => battleEffect;
    public Enemy EnemyToSpawn => enemyToSpawn;
    public Vector3 EnemySpawnOffset => enemySpawnOffset;
    public ActionConditionData ConditionData => conditionData;
    public AnimationData CharacterAnimation => characterAnimation;
    public AnimationData2 CharacterAnimation2 => characterAnimation2;
    public AtTargetAnimationData AtTargetAnimation => atTargetAnimation;
    
    public CardActionV2() {}
    public CardActionV2(EffectData e)
    {
        battleEffect = e;
    }

    public CardActionV2 Clone(EffectData dataToReplace) => new CardActionV2
    {
        type = type, 
        battleEffect = dataToReplace, 
        conditionData = conditionData,
        characterAnimation = characterAnimation, 
        atTargetAnimation = atTargetAnimation, 
        enemyToSpawn = enemyToSpawn,
        enemySpawnOffset = enemySpawnOffset
    };
}
