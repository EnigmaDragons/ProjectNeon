using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/DecklistEventPublisher")]
public class DecklistEventPublisher : ScriptableObject, ILocalizeTerms
{
    [SerializeField] private CardPlayZones playZones;

    public void ShowDeckCardsList() => Message.Publish(new ShowDeckList(playZones.DrawZone.Cards));
    public void ShowDiscardCardsList() => Message.Publish(new ShowDeckList(playZones.DiscardZone.Cards));
    public void ShowDeckCards() => Message.Publish(new ShowCards("BattleUI/DeckZoneCards", playZones.DrawZone.Cards));
    public void ShowDiscardCards() => Message.Publish(new ShowCards("BattleUI/DiscardZoneCards", playZones.DiscardZone.Cards));
    public void ToggleDecklistView() => Message.Publish(new ToggleDecklistView());
    public void ToggleCardsView() => Message.Publish(new ToggleCardsView());

    public string[] GetLocalizeTerms()
        => new[] {"BattleUI/DeckZoneCards", "BattleUI/DiscardZoneCards"};
}
