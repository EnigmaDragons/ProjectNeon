using UnityEngine;

public class BattleCharacterReactionSoundGuy : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    [SerializeField, FMODUnity.EventRef] private string onHpGainedEvent;
    [SerializeField, FMODUnity.EventRef] private string onCardFizzledBecauseStunnedEvent;
    [SerializeField, FMODUnity.EventRef] private string onChainCardPlayed;
    [SerializeField, FMODUnity.EventRef] private string onDeath;

    private bool debuggingLoggingEnabled = false;

    private void OnEnable()
    {
        Message.Subscribe<MemberStateChanged>(OnMemberStateChanged, this);
        Message.Subscribe<DisplayCharacterWordRequested>(OnCharacterReaction, this);
    }

    private void OnCharacterReaction(DisplayCharacterWordRequested msg)
    {
        if (msg.MemberId < 0)
            return;
        
        var memberTransform = battleState.GetTransform(msg.MemberId);
        if (msg.ReactionType == CharacterReactionType.Stunned)
            PlayOneShot(onCardFizzledBecauseStunnedEvent, memberTransform);
        if (msg.ReactionType == CharacterReactionType.ChainCardPlayed)
            PlayOneShot(onChainCardPlayed, memberTransform);
    }

    private void OnMemberStateChanged(MemberStateChanged msg)
    {
        if (msg.MemberId() < 0)
            return;
        
        var memberTransform = battleState.GetTransform(msg.MemberId());
        if (msg.GainedHp())
            PlayOneShot(onHpGainedEvent, memberTransform);
        if (msg.WasKnockedOut())
            PlayOneShot(onDeath, memberTransform);
    }

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