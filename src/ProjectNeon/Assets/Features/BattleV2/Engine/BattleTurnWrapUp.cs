using System.Collections;
using UnityEngine;

public class BattleTurnWrapUp : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZones zones;
    [SerializeField] private CardResolutionZone resolutionZone;
    
    public IEnumerator Execute()
    {
        resolutionZone.NotifyTurnFinished();
        yield return zones.DrawHandAsync(state.PlayerState.CurrentStats.CardDraw());
        Debug.Log("Began Advancing Turn");
        state.AdvanceTurn();
    }
}
