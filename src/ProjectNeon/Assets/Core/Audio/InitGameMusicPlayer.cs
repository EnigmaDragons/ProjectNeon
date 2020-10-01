using UnityEngine;

public sealed class InitGameMusicPlayer : CrossSceneSingleInstance
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private GameMusicPlayer musicPlayer;

    protected override void OnAwake() => musicPlayer.Init(musicSource);

    protected override string UniqueTag => "Music";
}
