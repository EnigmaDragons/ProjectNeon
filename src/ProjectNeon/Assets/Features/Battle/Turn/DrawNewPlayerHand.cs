using UnityEngine;

public sealed class DrawNewPlayerHand : GameEventActionScript
{
    [SerializeField] private CardPlayZones zones;

    private CardPlayZone Discard => zones.DiscardZone;
    private CardPlayZone Draw => zones.DrawZone;
    private CardPlayZone Hand => zones.HandZone;
    
    protected override void Execute()
    {
        while (!Hand.IsFull)
        {
            if (Draw.Count == 0)
                Reshuffle();
            Hand.PutOnBottom(Draw.DrawOneCard());
        }
    }

    private void Reshuffle()
    {
        while (Discard.Count > 0) 
            Draw.PutOnBottom(Discard.DrawOneCard());
        Draw.Shuffle();
    }
}
