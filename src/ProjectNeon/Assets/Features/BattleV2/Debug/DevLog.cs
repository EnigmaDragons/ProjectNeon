using UnityEngine;

public class DevLog : MonoBehaviour
{
    private static bool loggingEnabled = true;
    
    public static void Info(string message)
    {
        Write(message);
    }

    public static void Write(string message)
    {
        if (!loggingEnabled) return;
        
        Log.Info($"Battle (Dev) - {message}");
        Message.Publish(new WriteDevLogMessageRequested(message));
    }
}
