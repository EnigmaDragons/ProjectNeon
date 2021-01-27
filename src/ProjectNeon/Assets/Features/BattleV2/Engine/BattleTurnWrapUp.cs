using UnityEngine;

public class BattleTurnWrapUp : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZones zones;
    
    public void Execute()
    {
        zones.DrawHand(state.PlayerState.CurrentStats.CardDraw());
        state.AdvanceTurn();
    }
}
