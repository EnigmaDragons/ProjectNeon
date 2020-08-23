
using UnityEngine;

public class BattleTurnWrapUp : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZones zones;
    
    private CardPlayZone Draw => zones.DrawZone;
    private CardPlayZone Hand => zones.HandZone;
    
    public void Execute()
    {
        DrawNewPlayerHand();
        state.AdvanceTurn();
        state.Enemies.ForEach(e => e.State.GainPrimaryResource(1));
    }
    
    private void DrawNewPlayerHand()
    {
        while (!Hand.IsFull && Hand.Cards.Length < state.PlayerState.CurrentStats.CardDraw())
        {
            if (Draw.Count == 0)
                zones.Reshuffle();
            Hand.PutOnBottom(Draw.DrawOneCard());
        }
    }
}
