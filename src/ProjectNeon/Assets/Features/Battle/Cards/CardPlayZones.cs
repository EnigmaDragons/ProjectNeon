using UnityEngine;

public class CardPlayZones : ScriptableObject
{
    [SerializeField] private CardPlayZone drawZone;
    [SerializeField] private CardPlayZone handZone;
    [SerializeField] private CardPlayZone playZone;
    [SerializeField] private CardPlayZone discardZone;
    [SerializeField] private CardPlayZone selectionZone;

    public CardPlayZone DrawZone => drawZone;
    public CardPlayZone HandZone => handZone;
    public CardPlayZone PlayZone => playZone;
    public CardPlayZone DiscardZone => discardZone;
    public CardPlayZone SelectionZone => selectionZone;

    public void ClearAll()
    {
        drawZone.Clear();
        handZone.Clear();
        playZone.Clear();
        discardZone.Clear();
        selectionZone.Clear();
    }
}
