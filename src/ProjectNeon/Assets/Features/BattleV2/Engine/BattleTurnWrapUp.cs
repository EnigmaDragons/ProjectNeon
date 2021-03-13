using System.Collections;
using UnityEngine;

public class BattleTurnWrapUp : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZones zones;
    
    public IEnumerator Execute()
    {
        yield return zones.DrawHandAsync(state.PlayerState.CurrentStats.CardDraw());
        Debug.Log("Advancing Turn");
        state.AdvanceTurn();
    }
}
