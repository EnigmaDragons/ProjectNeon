using MoreMountains.Feedbacks;
using UnityEngine;

public class AlgeronShootsHelenCutsceneEvent : OnMessage<TriggerCutsceneEvent>
{
    [SerializeField] private int algeronMemberId;
    [SerializeField] private int helenMemberId;
    [SerializeField] private float delayBeforeHit = 0.3f;
    [SerializeField] private Enemy algeron;
    [SerializeField] private CharacterCreatorAnimationController algeronCharacter;
    [SerializeField] private DeathPresenter helenDeathPresenter;
    [SerializeField] private StringVariable triggeringEvent;
    [SerializeField, FMODUnity.EventRef] private string shootSound;
    [SerializeField] private float delayBeforeSound = 0.1f;
    [SerializeField] private AllBattleVfx fx;
    [SerializeField] private MMF_Player juiceFx;

    private void Start()
    {
        algeronCharacter.Init(algeronMemberId, algeron.Animations, TeamType.Enemies);
        helenDeathPresenter.Init(helenMemberId);
    }
    
    protected override void Execute(TriggerCutsceneEvent msg)
    {
        if (!msg.EventName.Equals(triggeringEvent.Value))
            return;
        
        Message.Publish(new CharacterAnimationRequested2(algeronMemberId, CharacterAnimationType.Shoot));
        this.ExecuteAfterDelay(delayBeforeSound, () =>
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(shootSound, algeronCharacter.gameObject);
        });
        this.ExecuteAfterDelay(delayBeforeHit, () =>
        {
            juiceFx.PlayFeedbacks();
            var deathParticleTarget = helenDeathPresenter.gameObject.GetComponentInChildren<CenterPoint>().transform.position;
            Instantiate(fx.ByName["Death"], deathParticleTarget, Quaternion.identity).Initialized();
            Message.Publish(
                new MemberUnconscious(new Member(helenMemberId, "", "", MemberMaterialType.Organic, TeamType.Enemies,
                    new StatAddends().With(StatType.Damagability, 1), BattleRole.Unknown, StatType.Power, false, false)));
        });
    }
}
