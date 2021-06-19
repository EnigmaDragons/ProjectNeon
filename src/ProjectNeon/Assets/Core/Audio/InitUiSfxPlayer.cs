using UnityEngine;

public sealed class InitUiSfxPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private UiSfxPlayer uiSfxPlayer;

    private void Awake() => uiSfxPlayer.InitIfNeeded(uiAudioSource);
}
