using System.Linq;
using UnityEngine;

public sealed class CheckForBattleFinished : GameEventActionScript
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameEvent onPlayerWin;
    [SerializeField] private GameEvent onPlayerLoss;
    
    protected override void Execute()
    {
        if (state.Heroes.All(m => !m.State.IsConscious))
            onPlayerLoss.Publish();
        else if (state.Enemies.All(m => !m.State.IsConscious))
            onPlayerWin.Publish();
    }
}
