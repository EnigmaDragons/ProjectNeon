using UnityEngine;

public sealed class SceneBackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private GameMusicPlayer musicPlayer;
    
    private void OnEnable()
    {
        musicPlayer.PlaySelectedMusicLooping(music);
    }
}
