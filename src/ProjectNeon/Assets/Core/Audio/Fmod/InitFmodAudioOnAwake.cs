
using UnityEngine;

public class InitFmodAudioOnAwake : MonoBehaviour
{
    [SerializeField] private FmodGameAudioManager fmodAudio;

    private void Awake()
    {
        fmodAudio.Init();
    }
}