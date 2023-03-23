using UnityEngine;

public sealed class SaveHandler : OnMessage<AutoSaveRequested, SaveDecksRequested, SaveDeterminationsRequested>
{
    [SerializeField] private SaveLoadSystem io;
    
    protected override void Execute(AutoSaveRequested msg)
    {
        io.SaveCheckpoint();
        CurrentGameOptions.Save();
    }
    
    protected override void Execute(SaveDecksRequested msg)
    {
        io.SaveDecks();
        CurrentGameOptions.Save();
    }
    
    protected override void Execute(SaveDeterminationsRequested msg)
    {
        io.SaveDeterminations();
        CurrentGameOptions.Save();
    }
}
