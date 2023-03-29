using UnityEngine;

public class SetSingleBattleTutorialToFalseOnEnable : MonoBehaviour
{
    [SerializeField] private BattleState battleState;

    private void Start() => battleState.IsSingleTutorialBattle = false;
}