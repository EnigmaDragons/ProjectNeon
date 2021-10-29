using UnityEngine;

public class BattleCharacterReactionSoundGuy : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    [SerializeField, FMODUnity.EventRef] private string onHpGainedEvent;
    [SerializeField, FMODUnity.EventRef] private string onCardFizzledBecauseStunnedEvent;
    [SerializeField, FMODUnity.EventRef] private string onCardChained;
    [SerializeField, FMODUnity.EventRef] private string onDeath;
    [SerializeField, FMODUnity.EventRef] private string onHPLost;

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
            if (msg.ReactionType == CharacterReactionType.Stunned)
                PlayOneShot(onCardFizzledBecauseStunnedEvent, memberTransform);
            if (msg.ReactionType == CharacterReactionType.ChainCardPlayed)
                PlayOneShot(onCardChained, memberTransform);
        });
    }

    private void OnMemberStateChanged(MemberStateChanged msg)
    {
        battleState.GetMaybeTransform(msg.MemberId()).IfPresent(memberTransform =>
        {
            if (msg.GainedHp())
                PlayOneShot(onHpGainedEvent, memberTransform);
            if (msg.LostHp())
                PlayOneShot(onHPLost, memberTransform);
            if (msg.WasKnockedOut())
                PlayOneShot(onDeath, memberTransform);
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