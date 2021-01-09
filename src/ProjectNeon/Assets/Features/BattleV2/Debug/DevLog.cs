using UnityEngine;

public class DevLog : OnMessage<WriteDevLogMessageRequested>
{
    [SerializeField] private BoolReference loggingEnabled;
    
    protected override void Execute(WriteDevLogMessageRequested e)
    {
        if (loggingEnabled)
            Log.Info($"Battle (Dev) - {e.Message}");
    }

    public static void Info(string message) => Write(message);
    public static void Write(string message) => Message.Publish(new WriteDevLogMessageRequested(message)); 
}
