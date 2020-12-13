using UnityEngine;

public class UpdatePartyAdventureStateHandler : OnMessage<UpdatePartyAdventureState>
{
    [SerializeField] private PartyAdventureState party;
    
    protected override void Execute(UpdatePartyAdventureState msg)
    {
        msg.ApplyTo(party);
    }
}
