using UnityEngine;

public class BattleLog : OnBattleEvent<WriteBattleLogMessageRequested>
{
    [SerializeField] private BoolReference loggingEnabled;
    
    protected override void Execute(WriteBattleLogMessageRequested e)
    {
        if (loggingEnabled)
            Debug.Log($"Battle - {e.Message}");
    }
    
    
    public static void Write(string message) => BattleEvent.Publish(new WriteBattleLogMessageRequested(message)); 
}



