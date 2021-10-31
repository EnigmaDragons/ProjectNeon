using System;
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
    
    public static void WriteIf<T>(T val, Func<T, string> getMessage, Func<T, bool> condition)
    {
        if (condition(val))
            Message.Publish(new WriteBattleLogMessageRequested(getMessage(val)));
    }
    
    public static string GainedOrLostTerm(float amount) => amount > 0 ? "gained" : "lost";
}
