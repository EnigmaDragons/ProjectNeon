using UnityEngine;

public class SimpleCardsUIController : OnMessage<ToggleCardsView, ShowCards>
{
    [SerializeField] private GameObject uiObj;
    [SerializeField] private SimpleCardsUI cardUi;

    private void Awake() => uiObj.SetActive(false);
    
    protected override void Execute(ToggleCardsView msg)
    {
        uiObj.SetActive(!uiObj.activeSelf);
    }

    protected override void Execute(ShowCards msg)
    {
        cardUi.Init($"{msg.CardZoneName} Cards", msg.Cards);
        uiObj.SetActive(true);
    }
}
