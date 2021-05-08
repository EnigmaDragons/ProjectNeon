using UnityEngine;

public sealed class SceneBackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private GameMusicPlayer musicPlayer;
    
    private void Start()
    {
        this.ExecuteAfterTinyDelay(() =>  musicPlayer.PlaySelectedMusicLooping(music));
    }
}
