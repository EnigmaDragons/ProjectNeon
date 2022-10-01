using UnityEngine;

public class DecklistUIController : OnMessage<ShowDeckList, ToggleDecklistView>
{
    [SerializeField] private GameObject uiObj;
    [SerializeField] private SimpleDeckUI deckUi;

    private void Awake() => uiObj.SetActive(false);

    public void ShowDeckList(Card[] cards)
    {
        deckUi.Init(cards);
        uiObj.SetActive(true);
    }
    
    protected override void Execute(ShowDeckList msg)
    {
        ShowDeckList(msg.Cards);
    }

    protected override void Execute(ToggleDecklistView msg)
    {
        uiObj.SetActive(!uiObj.activeSelf);
    }
}
