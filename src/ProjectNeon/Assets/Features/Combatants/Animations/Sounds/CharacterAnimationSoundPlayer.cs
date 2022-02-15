using UnityEngine;

public class CharacterAnimationSoundPlayer : OnMessage<CharacterAnimationRequested2, Finished<CharacterAnimationRequested2>, PlayCharacterAnimSound>
{
    [SerializeField] private bool alwaysActive = false;
    
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

    public void SetAlwaysActive(bool val) => alwaysActive = val;
    
    private bool IsInitialized() => _sounds != null && _uiSource != null;
    
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
        if (!IsInitialized() || (!_active && !alwaysActive))
        {
            Log.Info($"{nameof(PlayCharacterAnimSound)} IsInitialized: {IsInitialized()}, IsActive {_active || alwaysActive}");
            return;
        }

        _sounds.Play(_uiSource, msg.SoundType);
    }
}
