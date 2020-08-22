using UnityEngine;

public class OnAwakeGiveAllEffectsACopyOfBattleState : MonoBehaviour
{
    [SerializeField] private BattleState battleState;

    private void Awake() => AllEffects.InitBattleState(battleState);
}