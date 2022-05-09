using UnityEngine;

public class BasicSuperFocusHandler : OnMessage<SetSuperFocusBasicControl>
{
    [SerializeField] private BattleState state;
    
    protected override void Execute(SetSuperFocusBasicControl msg) => state.SetBasicSuperFocusEnabled(msg.Enabled);
}
