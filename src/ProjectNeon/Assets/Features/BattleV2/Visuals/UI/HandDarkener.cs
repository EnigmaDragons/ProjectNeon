using UnityEngine;

public class HandDarkener : OnMessage<BattleStateChanged, PlayerTurnConfirmed>
{
    [SerializeField] private GameObject target;

    private void Awake() => target.SetActive(false);
    protected override void Execute(BattleStateChanged msg)
    {
        if (msg.State.Phase == BattleV2Phase.Wrapup || msg.State.Phase == BattleV2Phase.EndOfTurnEffects)
            target.SetActive(false);
    }

    protected override void Execute(PlayerTurnConfirmed msg) => target.SetActive(true);
}
