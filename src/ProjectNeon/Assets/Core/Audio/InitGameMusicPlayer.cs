using UnityEngine;

public sealed class InitGameMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private GameMusicPlayer musicPlayer;
    
    private void Awake()
    {
        var objs = GameObject.FindGameObjectsWithTag("Music");
        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        musicPlayer.Init(musicSource);
        DontDestroyOnLoad(gameObject);
    }
}
