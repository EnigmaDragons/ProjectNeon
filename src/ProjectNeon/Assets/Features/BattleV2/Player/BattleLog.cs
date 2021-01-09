using UnityEngine;

public class BattleLog : OnMessage<WriteBattleLogMessageRequested>
{
    [SerializeField] private BoolReference loggingEnabled;

    protected override void Execute(WriteBattleLogMessageRequested e)
    {
        if (loggingEnabled)
            Log.Info($"Battle - {e.Message}");
    }

    public static void Write(string message) => Message.Publish(new WriteBattleLogMessageRequested(message)); 
}
