using UnityEngine;

public class BattleCharacterReactionSoundGuy : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    [SerializeField, FMODUnity.EventRef] private string onHpGainedEvent;
    [SerializeField, FMODUnity.EventRef] private string onCardFizzledBecauseStunnedEvent;
    [SerializeField, FMODUnity.EventRef] private string onDeath;
    [SerializeField, FMODUnity.EventRef] private string onShieldGained;
    [SerializeField, FMODUnity.EventRef] private string onShieldLost;
    [SerializeField, FMODUnity.EventRef] private string onStealthGained;
    [SerializeField, FMODUnity.EventRef] private string onStealthExited;
    [SerializeField, FMODUnity.EventRef] private string onBonusCardPlayed;
    [SerializeField, FMODUnity.EventRef] private string onAegised;
    [SerializeField, FMODUnity.EventRef] private string onCardChained;

    private bool debuggingLoggingEnabled = false;

    private void OnEnable()
    {
        Message.Subscribe<MemberStateChanged>(OnMemberStateChanged, this);
        Message.Subscribe<DisplayCharacterWordRequested>(OnCharacterReaction, this);
    }

    private void OnCharacterReaction(DisplayCharacterWordRequested msg)
    {
        battleState.GetMaybeTransform(msg.MemberId).IfPresent(memberTransform =>
        {
            if (msg.ReactionType == CharacterReactionType.ChainCardPlayed)
                PlayOneShot(onCardChained, memberTransform);
            Debug.Log("CHAIN_SFX_Playing");
            if (msg.ReactionType == CharacterReactionType.Stunned)
                 PlayOneShot(onCardFizzledBecauseStunnedEvent, memberTransform);
             if (msg.ReactionType == CharacterReactionType.BonusCardPlayed)
                 PlayOneShot(onBonusCardPlayed, memberTransform);
            if (msg.ReactionType == CharacterReactionType.Aegised)
                PlayOneShot(onAegised, memberTransform);
            Debug.Log("AEGISED_SFX_Playing");

        });
    }

    private void OnMemberStateChanged(MemberStateChanged msg)
    {
        battleState.GetMaybeTransform(msg.MemberId()).IfPresent(memberTransform =>
        {
            if (msg.GainedHp())
                PlayOneShot(onHpGainedEvent, memberTransform);
            if (msg.WasKnockedOut())
                PlayOneShot(onDeath, memberTransform);
            if (msg.GainedShield())
                PlayOneShot(onShieldGained, memberTransform);
            if (msg.LostShield())
                PlayOneShot(onShieldLost, memberTransform);
            if (msg.GainedStealth())
                PlayOneShot(onStealthGained, memberTransform);
            if (msg.ExitedStealth())
                PlayOneShot(onStealthExited, memberTransform);
        });
    }

    private MemberMaterialType MaterialTypeOf(int memberId) => battleState.MaterialTypeOf(memberId);

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
    
    private void PlayOneShot(string eventName, Transform memberTransform)
        => FMODUnity.RuntimeManager.PlayOneShot(eventName, memberTransform.position);
    
    private void DebugLog(string msg)
    {
        if (debuggingLoggingEnabled)
            Log.Info(msg);
    }
    
}