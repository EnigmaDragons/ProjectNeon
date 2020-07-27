using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleVFXController : OnMessage<BattleEffectAnimationRequested>
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleVFX[] fx;
    [SerializeField] private Transform enemyGroupLocation;
    [SerializeField] private Transform heroesGroupLocation;

    private void Awake() => fx.ForEach(f => f.gameObject.SetActive(false));

    protected override void Execute(BattleEffectAnimationRequested e)
    {
        Debug.Log($"VFX Requested {e.EffectName}");
        var f = fx.FirstOrDefault(x => x.EffectName.Equals(e.EffectName));
        if (f == null)
            Debug.Log($"No VFX of type {e.EffectName}");
        else if (e.Scope.Equals(Scope.One) || e.Group == Group.Self)
        {
            var location = state.GetTransform(e.PerformerId);
            PlayEffect(f, location);
        }
        else if (e.Group == Group.All)
            Debug.Log($"All Characters VFX not supported yet");
        else
        {
            var performerTeam = state.Members[e.PerformerId].TeamType;
            var opponentTeam = performerTeam == TeamType.Enemies ? TeamType.Party : TeamType.Enemies;
            var targetTeam = e.Group == Group.Opponent ? opponentTeam : performerTeam;
            var location = targetTeam == TeamType.Enemies ? enemyGroupLocation : heroesGroupLocation;
            PlayEffect(f, location);
        }
    }

    private void PlayEffect(BattleVFX f, Transform location)
    {
        var o = Instantiate(f.gameObject, location, gameObject);
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