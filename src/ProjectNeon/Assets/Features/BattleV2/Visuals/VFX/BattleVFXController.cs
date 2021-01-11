using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleVFXController : OnMessage<BattleEffectAnimationRequested, PlayRawBattleEffect>
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleVFX[] fx;
    [SerializeField] private Transform enemyGroupLocation;
    [SerializeField] private Transform heroesGroupLocation;

    private void Awake()
    {
        foreach (var f in fx)
            if (f.EffectName.Equals(""))
                Log.Error($"{f.name} is missing it's EffectName");
    }
    
    protected override void Execute(BattleEffectAnimationRequested e)
    {
        Log.Info($"VFX Requested {e.EffectName}");
        var f = fx.FirstOrDefault(x => x.EffectName.Equals(e.EffectName));
        if (f == null)
        {
            Log.Info($"No VFX of type {e.EffectName}");
            Message.Publish(new Finished<BattleEffectAnimationRequested>());
        }
        else if (e.Scope.Equals(Scope.One))
        {
            var location = state.GetCenterPoint(e.Target.Members[0].Id);
            PlayEffect(f, location.position, location, e.Size, e.Speed, e.Color);
        }
        else if (e.Group == Group.All)
            Log.Info($"All Characters VFX not supported yet");
        else
        {
            var performerTeam = state.Members[e.PerformerId].TeamType;
            var opponentTeam = performerTeam == TeamType.Enemies ? TeamType.Party : TeamType.Enemies;
            var targetTeam = e.Group == Group.Opponent ? opponentTeam : performerTeam;
            var location = targetTeam == TeamType.Enemies ? enemyGroupLocation : heroesGroupLocation;
            PlayEffect(f, location.position, location, e.Size, e.Speed, e.Color);
        }
    }

    protected override void Execute(PlayRawBattleEffect e)
    {        
        Log.Info($"VFX Requested {e.EffectName}");
        var f = fx.FirstOrDefault(x => x.EffectName.Equals(e.EffectName));
        if (f == null)
            Log.Info($"No VFX of type {e.EffectName}");
        
        PlayEffect(f, e.Target, gameObject.transform, 1, 1, new Color(0, 0, 0, 0));
    }

    private void PlayEffect(BattleVFX f, Vector3 target, Transform parent, float size, float speed, Color color)
    {
        var o = Instantiate(f.gameObject, target, f.gameObject.transform.rotation, parent);
        var instVFX = o.GetComponent<BattleVFX>();
        SetupEffect(o, instVFX, size, speed, color, parent.lossyScale);
        if (instVFX.WaitForCompletion)
            StartCoroutine(AwaitAnimationFinish(instVFX));
        else
            Message.Publish(new Finished<BattleEffectAnimationRequested>());
    }

    private void SetupEffect(GameObject o, BattleVFX f, float size, float speed, Color color, Vector3 parentScale)
    {
        var effectObject = o.transform.GetChild(0);
        f.SetSpeed(speed);
        effectObject.localScale = new Vector3(size / parentScale.x, size / parentScale.y, size / parentScale.z);
        effectObject.localPosition *= size;
        if (speed != 1 || color.a > 0)
        {
            Color.RGBToHSV(color, out var hue, out var saturation, out var _);
            var particleSystems = o.GetComponentsInChildren<ParticleSystem>();
            foreach (var system in particleSystems)
            {
                ParticleSystem.MainModule main = system.main;
                if (speed != 1)
                    main.simulationSpeed = speed;
                if (color.a > 0)
                {
                    Color.RGBToHSV(main.startColor.color, out var _, out var _, out var value);
                    var newColor = Color.HSVToRGB(hue, saturation, value);
                    newColor.a = main.startColor.color.a;
                    main.startColor = new ParticleSystem.MinMaxGradient(newColor);   
                }
            }
        }
        o.SetActive(true);
    }

    private IEnumerator AwaitAnimationFinish(BattleVFX f)
    {
        yield return new WaitForSeconds(f.DurationSeconds);
        DevLog.Write($"Finished {f.EffectName} in {f.DurationSeconds} seconds.");
        Message.Publish(new Finished<BattleEffectAnimationRequested>());
    }
}
