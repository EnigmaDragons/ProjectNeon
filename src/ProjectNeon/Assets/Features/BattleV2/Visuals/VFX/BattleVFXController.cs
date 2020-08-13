using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleVFXController : OnMessage<BattleEffectAnimationRequested, PlayRawBattleEffect>
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleVFX[] fx;
    [SerializeField] private Transform enemyGroupLocation;
    [SerializeField] private Transform heroesGroupLocation;

    private void Awake() => fx.ForEach(f => f.gameObject.SetActive(false));

    protected override void Execute(BattleEffectAnimationRequested e)
    {
        Log.Info($"VFX Requested {e.EffectName}");
        var f = fx.FirstOrDefault(x => x.EffectName.Equals(e.EffectName));
        if (f == null)
            Log.Info($"No VFX of type {e.EffectName}");
        else if (e.Scope.Equals(Scope.One) || e.Group == Group.Self)
        {
            var location = state.GetTransform(e.PerformerId);
            PlayEffect(f, location.position);
        }
        else if (e.Group == Group.All)
            Log.Info($"All Characters VFX not supported yet");
        else
        {
            var performerTeam = state.Members[e.PerformerId].TeamType;
            var opponentTeam = performerTeam == TeamType.Enemies ? TeamType.Party : TeamType.Enemies;
            var targetTeam = e.Group == Group.Opponent ? opponentTeam : performerTeam;
            var location = targetTeam == TeamType.Enemies ? enemyGroupLocation : heroesGroupLocation;
            PlayEffect(f, location.position);
        }
    }

    protected override void Execute(PlayRawBattleEffect e)
    {        
        Log.Info($"VFX Requested {e.EffectName}");
        var f = fx.FirstOrDefault(x => x.EffectName.Equals(e.EffectName));
        if (f == null)
            Log.Info($"No VFX of type {e.EffectName}");
        
        PlayEffect(f, e.Target);
    }

    private void PlayEffect(BattleVFX f, Vector3 target)
    {
        var o = Instantiate(f.gameObject, target, Quaternion.identity, gameObject.transform);
        o.SetActive(true);
        if (f.WaitForCompletion)
            StartCoroutine(AwaitAnimationFinish(f));
        else
            Message.Publish(new Finished<BattleEffectAnimationRequested>());
    }
    
    private IEnumerator AwaitAnimationFinish(BattleVFX f)
    {
        yield return new WaitForSeconds(f.DurationSeconds);
        BattleLog.Write($"Finished {f.EffectName} in {f.DurationSeconds} seconds.");
        Message.Publish(new Finished<BattleEffectAnimationRequested>());
    }
}
