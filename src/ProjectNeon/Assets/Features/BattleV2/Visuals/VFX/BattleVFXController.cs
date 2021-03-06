using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleVFXController : OnMessage<BattleEffectAnimationRequested, PlayRawBattleEffect>
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleVFX[] fx;
    [SerializeField] private Transform enemyGroupLocation;
    [SerializeField] private Transform heroesGroupLocation;
    [SerializeField] private bool loggingEnabled = false;

    private void Awake()
    {
        foreach (var f in fx)
            if (f.EffectName.Equals(""))
                Log.Error($"{f.name} is missing it's EffectName");
    }
    
    protected override void Execute(BattleEffectAnimationRequested e)
    {
        LogInfo($"Requested VFX {e.EffectName}");
        var f = fx.FirstOrDefault(x => x.EffectName.Equals(e.EffectName));
        if (f == null)
        {
            LogInfo($"No VFX of type {e.EffectName}");
            Message.Publish(new Finished<BattleEffectAnimationRequested>());
        }
        else if (e.Scope.Equals(Scope.One))
        {
            var member = e.Target.Members[0];
            if (!member.IsConscious())
            {
                Message.Publish(new Finished<BattleEffectAnimationRequested>());
                return;
            }

            var location = state.GetCenterPoint(member.Id);
            PlayEffect(f, location.position, location, e.Size, e.Speed, e.Color, e.Target.Members[0].TeamType == TeamType.Enemies);
        }
        else if (e.Group == Group.All)
        {
            LogInfo($"All Characters VFX not supported yet");
            Message.Publish(new Finished<BattleEffectAnimationRequested>());
        }
        else
        {
            var performerTeam = state.Members[e.PerformerId].TeamType;
            var opponentTeam = performerTeam == TeamType.Enemies ? TeamType.Party : TeamType.Enemies;
            var targetTeam = e.Group == Group.Opponent ? opponentTeam : performerTeam;
            var location = targetTeam == TeamType.Enemies ? enemyGroupLocation : heroesGroupLocation;
            PlayEffect(f, location.position, location, e.Size, e.Speed, e.Color, e.Target.Members.All(x => x.TeamType == TeamType.Enemies));
        }
    }

    protected override void Execute(PlayRawBattleEffect e)
    {        
        LogInfo($"Requested VFX {e.EffectName}");
        var f = fx.FirstOrDefault(x => x.EffectName.Equals(e.EffectName));
        if (f == null)
            LogInfo($"No VFX of type {e.EffectName}");
        
        PlayEffect(f, e.Target, gameObject.transform, 1, 1, new Color(0, 0, 0, 0), false);
    }

    private void PlayEffect(BattleVFX f, Vector3 target, Transform parent, float size, float speed, Color color, bool shouldFlipHorizontal)
    {
        var o = Instantiate(f.gameObject, target + f.gameObject.transform.localPosition, f.gameObject.transform.rotation, parent);
        var instVFX = o.GetComponent<BattleVFX>();
        SetupEffect(o, instVFX, size, speed, color, shouldFlipHorizontal);
        if (instVFX.WaitForCompletion)
            StartCoroutine(AwaitAnimationFinish(instVFX));
        else
            Message.Publish(new Finished<BattleEffectAnimationRequested>());
    }

    private void SetupEffect(GameObject o, BattleVFX f, float size, float speed, Color color, bool shouldFlipHorizontal)
    {
        var effectObject = o.transform.GetChild(0);
        f.SetSpeed(speed);
        effectObject.localScale = new Vector3(size, size, size);
        effectObject.localPosition = new Vector3(effectObject.localPosition.x * size, effectObject.localPosition.y * size, effectObject.localPosition.z * size);
        if (shouldFlipHorizontal)
        {
            effectObject.localPosition = new Vector3(-effectObject.localPosition.x, effectObject.localPosition.y, effectObject.localPosition.z);
            effectObject.transform.Rotate(0, 180f, 0, Space.World);
        }
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
        LogInfo($"Finished {f.EffectName} in {f.DurationSeconds} seconds.");
        Message.Publish(new Finished<BattleEffectAnimationRequested>());
    }

    private void LogInfo(string msg)
    {
        if (loggingEnabled)
            Log.Info("VFX - " + msg);
    }
}
