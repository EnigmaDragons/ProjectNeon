using UnityEngine;

public class InitFmodAudio : MonoBehaviour
{
    [SerializeField] private FmodGameAudioManager fmodAudio;

    private void Awake() => fmodAudio.Init();
}
