using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurnWrapUpPhase : MonoBehaviour
{
    [SerializeField] private GameEvent battleTurnWrapUpStarted;
    [SerializeField] private GameEvent battleTurnWrapUpFinished;
    [SerializeField] private GameEvent playerTurnConfirmed;
    [SerializeField] private GameEvent turnStarted;

    private void OnEnable()
    {
        playerTurnConfirmed.Subscribe(StartBattleTurnPhase, this);
    }
    void StartBattleTurnPhase()
    {
        battleTurnWrapUpStarted.Publish();
        print("StartPhase");
    }
    [ContextMenu("Finish phase")]
    void FinishBattleTurnPhase()
    {
        battleTurnWrapUpFinished.Publish();
        // @todo #311: 30min Add re-setup battle cards
        turnStarted.Publish();
    }
}
