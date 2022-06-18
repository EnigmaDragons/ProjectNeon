using UnityEngine;

public class DecklistUIController : OnMessage<ShowDeckList, ToggleDecklistView>
{
    [SerializeField] private GameObject uiObj;
    [SerializeField] private SimpleDeckUI deckUi;

    private void Awake() => uiObj.SetActive(false);
    
    protected override void Execute(ShowDeckList msg)
    {
        deckUi.Init(msg.Cards);
        uiObj.SetActive(true);
    }

    protected override void Execute(ToggleDecklistView msg)
    {
        uiObj.SetActive(!uiObj.activeSelf);
    }
}
