using UnityEngine;

[CreateAssetMenu(menuName = "Battle/CardPlayZones")]
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

    public void Reshuffle()
    {
        while (DiscardZone.Count > 0) 
            DrawZone.PutOnBottom(DiscardZone.DrawOneCard());
        DrawZone.Shuffle();
    }
}
