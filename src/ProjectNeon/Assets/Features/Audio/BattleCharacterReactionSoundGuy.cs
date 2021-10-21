using UnityEngine;

public class BattleCharacterReactionSoundGuy : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    [SerializeField, FMODUnity.EventRef] private string onHpGainedEvent;
    [SerializeField, FMODUnity.EventRef] private string onCardFizzledBecauseStunnedEvent;
    
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
        });
    }

    private void OnMemberStateChanged(MemberStateChanged msg)
    {
        battleState.GetMaybeTransform(msg.MemberId()).IfPresent(memberTransform =>
        {
            if (msg.GainedHp())
                PlayOneShot(onHpGainedEvent, memberTransform);
        });
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