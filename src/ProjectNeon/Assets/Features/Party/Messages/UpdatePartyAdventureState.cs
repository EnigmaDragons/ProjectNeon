using System;

public class UpdatePartyAdventureState
{
    public Action<PartyAdventureState> ApplyTo { get; }

    public UpdatePartyAdventureState(Action<PartyAdventureState> action) => ApplyTo = action;
}
