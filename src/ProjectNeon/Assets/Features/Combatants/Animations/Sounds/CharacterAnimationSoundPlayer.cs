using UnityEngine;

public class CharacterAnimationSoundPlayer : OnMessage<CharacterAnimationRequested2, Finished<CharacterAnimationRequested2>, PlayCharacterAnimSound>
{
    private int _memberId;
    private CharacterAnimationSoundSet _sounds;
    private Transform _uiSource;
    private bool _active;
    
    public void Init(int memberId, CharacterAnimationSoundSet sounds, Transform uiSource)
    {
        _memberId = memberId;
        _sounds = sounds;
        _uiSource = uiSource;
        Log.Info("Init " + nameof(CharacterAnimationSoundPlayer));
    }

    private bool IsInitialized() => _memberId > -1 && _sounds != null && _uiSource != null;
    
    protected override void Execute(CharacterAnimationRequested2 msg)
    {
        if (!IsInitialized() || _memberId != msg.MemberId)
            return;

        _active = true;
    }

    protected override void Execute(Finished<CharacterAnimationRequested2> msg)
    {
        if (_active && msg.Message.MemberId == _memberId)
            _active = false;
    }

    protected override void Execute(PlayCharacterAnimSound msg)
    {
        if (!_active)
            return;
        
        _sounds.Play(_uiSource, msg.SoundType);
    }
}
