using Features.GameProgression.Messages;
using UnityEngine;

public sealed class SaveHandler : OnMessage<AutoSaveRequested>
{
    [SerializeField] private SaveLoadSystem io;
    
    protected override void Execute(AutoSaveRequested msg) => io.SaveCheckpoint();
}
