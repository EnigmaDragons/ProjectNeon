using UnityEngine;
using UnityEngine.UI;

public class CardShopDoneButtonController : OnMessage<SetSuperFocusBuyControl>
{
    [SerializeField] private Button doneButton;
    [SerializeField] private PartyAdventureState party;
    
    protected override void Execute(SetSuperFocusBuyControl msg) => doneButton.interactable = !msg.Enabled || party.Credits < 40;
}
