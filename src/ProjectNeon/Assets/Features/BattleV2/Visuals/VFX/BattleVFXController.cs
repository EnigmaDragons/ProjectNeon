using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleVFXController : OnMessage<BattleEffectAnimationRequested, PlayRawBattleEffect>
{
    [SerializeField] private BattleState state;
    [SerializeField] private AllBattleVfx fx;
    [SerializeField] private Transform enemyGroupLocation;
    [SerializeField] private Transform heroesGroupLocation;
    [SerializeField] private Transform effectsParent;
    [SerializeField] private PartyAdventureState partyAdventureState;

    private bool _loggingEnabled = false;
    private Dictionary<string, BattleVFX> _fxByName;
    
    private void Awake()
    {
        _fxByName = fx.ByName;
        foreach (var f in _fxByName)
            if (f.Value.EffectName.Equals(""))
                Log.Error($"{f.Value.name} is missing it's EffectName");
    }
    
    protected override void Execute(BattleEffectAnimationRequested e)
    {
        if (_loggingEnabled)
            LogInfo($"Requested VFX {e.EffectName}");
        var f = _fxByName.ValueOrMaybe(e.EffectName);
        var ctx = new EffectContext(e.Source, e.Target, e.Card, e.XPaidAmount, e.PaidAmount, partyAdventureState, state.PlayerState, state.RewardState,
            state.Members, state.PlayerCardZones, new UnpreventableContext(), new SelectionContext(), new Dictionary<int, CardTypeData>(), state.CreditsAtStartOfBattle, 
            state.Party.Credits, state.Enemies.ToDictionary(x => x.Member.Id, x => (EnemyType)x.Enemy), () => state.GetNextCardId(), 
            state.CurrentTurnCardPlays(), state.OwnerTints, state.OwnerBusts, false, ReactionTimingWindow.NotApplicable, new EffectScopedData());
        var conditionResult = e.Condition.GetShouldNotApplyReason(ctx);
        if (conditionResult.IsPresent)
        {
            if (_loggingEnabled)
                LogInfo($"Condition Not Meant: {conditionResult.Value}");
            Message.Publish(new Finished<BattleEffectAnimationRequested>());
        }
        else if (f.IsMissing)
        {
            Log.Warn($"No VFX of type {e.EffectName}");
            Message.Publish(new Finished<BattleEffectAnimationRequested>());
        }
        else if (e.Scope.Equals(Scope.One) || e.Scope.Equals(Scope.OneExceptSelf) || e.Scope.Equals(Scope.Random) || e.Scope.Equals(Scope.RandomExceptTarget))
        {
            if (e.Target.Members.None())
            {
                Message.Publish(new Finished<BattleEffectAnimationRequested>());
                return;
            }
            var member = e.Target.Members[0];
            if (!member.IsConscious() || !state.Members.ContainsKey(member.Id))
            {
                Message.Publish(new Finished<BattleEffectAnimationRequested>());
                return;
            }

            var maybeCenter = state.GetMaybeCenterPoint(member.Id);
            if (maybeCenter.IsMissing)
            {
                Message.Publish(new Finished<BattleEffectAnimationRequested>());
                return;
            }
            
            PlayEffect(f.Value, maybeCenter.Value.position, e.Size, e.Speed, e.Color, member.TeamType == TeamType.Enemies, e.SkipWaitingForCompletion);
        }
        else if (e.Group == Group.All)
        {
            PlayEffect(f.Value, state.GetCenterPoint(new Multiple(state.Members.Values)), e.Size, e.Speed, e.Color, false, e.SkipWaitingForCompletion);
        }
        else
        {
            var performerTeam = e.PerformerId == AllGlobalEffects.GlobalEffectMemberId 
                ? TeamType.Party
                : state.Members[e.PerformerId].TeamType;
            var opponentTeam = performerTeam == TeamType.Enemies ? TeamType.Party : TeamType.Enemies;
            var targetTeam = e.Group == Group.Opponent ? opponentTeam : performerTeam;
            var location = state.GetCenterPoint(targetTeam);
            PlayEffect(f.Value, location, e.Size, e.Speed, e.Color, e.Target.Members.All(x => x.TeamType == TeamType.Enemies), e.SkipWaitingForCompletion);
        }
    }

    protected override void Execute(PlayRawBattleEffect e)
    {        
        LogInfo($"Requested VFX {e.EffectName}");
        var f = _fxByName.ValueOrMaybe(e.EffectName);
        if (f.IsMissing)
            LogInfo($"No VFX of type {e.EffectName}");
        else
        {
            PlayEffect(f.Value, e.Target, 1, 1, new Color(0, 0, 0, 0), e.Flip, false);
        }
    }

    private void PlayEffect(BattleVFX f, Vector3 target, float size, float speed, Color color, bool shouldFlipHorizontal, bool skipWaitingForCompletion)
    {
        var o = Instantiate(f.gameObject, target + f.gameObject.transform.localPosition, f.gameObject.transform.rotation, effectsParent);
        var instVFX = o.GetComponent<BattleVFX>();
        SetupEffect(o, instVFX, size, speed, color, shouldFlipHorizontal);
        if (!skipWaitingForCompletion && instVFX.WaitForCompletion)
            StartCoroutine(AwaitAnimationFinish(instVFX, new WaitForSeconds(f.DurationSeconds)));
        else
            Message.Publish(new Finished<BattleEffectAnimationRequested>());
    }

    private void SetupEffect(GameObject o, BattleVFX f, float size, float speed, Color color, bool shouldFlipHorizontal)
    {
        var effectObject = o.transform.childCount > 0 ? o.transform.GetChild(0) : o.transform;
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
    
    private IEnumerator AwaitAnimationFinish(BattleVFX f, WaitForSeconds w)
    {
        yield return w;
        if (_loggingEnabled)
            LogInfo($"Finished {f.EffectName} in {f.DurationSeconds} seconds.");
        this.ExecuteAfterTinyDelay(() => Message.Publish(new Finished<BattleEffectAnimationRequested>()));
    }

    private void LogInfo(string msg)
    {
        if (_loggingEnabled)
            Log.Info("VFX - " + msg);
    }
}
