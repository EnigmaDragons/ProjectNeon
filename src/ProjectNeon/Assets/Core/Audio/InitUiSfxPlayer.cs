using UnityEngine;

public sealed class InitUiSfxPlayer : CrossSceneSingleInstance
{
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private UiSfxPlayer uiSfxPlayer;

    protected override string UniqueTag => "UiSounds";
    protected override void OnAwake() => uiSfxPlayer.Init(uiAudioSource);
}
