using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/DecklistEventPublisher")]
public class DecklistEventPublisher : ScriptableObject
{
    [SerializeField] private CardPlayZones playZones;

    public void ShowDeckCardsList() => Message.Publish(new ShowDeckList(playZones.DrawZone.Cards));
    public void ShowDiscardCardsList() => Message.Publish(new ShowDeckList(playZones.DiscardZone.Cards));
    public void ShowDeckCards() => Message.Publish(new ShowCards("Deck Zone", playZones.DrawZone.Cards));
    public void ShowDiscardCards() => Message.Publish(new ShowCards("Discard Zone", playZones.DiscardZone.Cards));
    public void ToggleDecklistView() => Message.Publish(new ToggleDecklistView());
    public void ToggleCardsView() => Message.Publish(new ToggleCardsView());
}
