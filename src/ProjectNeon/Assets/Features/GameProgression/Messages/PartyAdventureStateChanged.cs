
public sealed class PartyAdventureStateChanged
{
    public PartyAdventureState State { get; }

    public PartyAdventureStateChanged(PartyAdventureState s) => State = s;
}
