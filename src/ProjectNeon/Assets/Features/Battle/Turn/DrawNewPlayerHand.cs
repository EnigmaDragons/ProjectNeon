using UnityEngine;

public sealed class DrawNewPlayerHand : GameEventActionScript
{
    [SerializeField] private CardPlayZones zones;
    
    protected override void Execute()
    {
        while(!zones.HandZone.IsFull)
            zones.HandZone.PutOnBottom(zones.DrawZone.DrawOneCard());
    }
}
