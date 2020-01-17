using System.Linq;
using UnityEngine;

public class BattleVFXController : OnBattleEvent<BattleEffectAnimationRequested>
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleVFX[] fx;
    [SerializeField] private Transform enemyGroupLocation;
    [SerializeField] private Transform heroesGroupLocation;
    
    protected override void Execute(BattleEffectAnimationRequested e)
    {
        Debug.Log($"VFX Requested {e.EffectName}");
        var f = fx.FirstOrDefault(x => x.Name == e.EffectName);
        if (f == null)
            Debug.Log($"No VFX of type {e.EffectName}");
        else if (e.Scope.Equals(Scope.One) || e.Group == Group.Self)
        {
            var location = state.GetTransform(e.PerformerId);
            f.Effect.SetActive(false);
            f.Effect.transform.position = location.position;
            f.Effect.SetActive(true);
        }
        else if (e.Group == Group.All)
            Debug.Log($"All Characters VFX not supported yet");
        else
        {
            var performerTeam = state.Members[e.PerformerId].TeamType;
            var opponentTeam = performerTeam == TeamType.Enemies ? TeamType.Party : TeamType.Enemies;
            var targetTeam = e.Group == Group.Opponent ? opponentTeam : performerTeam;
            var location = targetTeam == TeamType.Enemies ? enemyGroupLocation : heroesGroupLocation;
            f.Effect.SetActive(false);
            f.Effect.transform.position = location.position;
            f.Effect.SetActive(true);
        }
    }
}