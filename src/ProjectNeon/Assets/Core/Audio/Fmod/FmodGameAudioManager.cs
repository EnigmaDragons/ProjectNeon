using FMOD.Studio;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/FmodGameAudioManager")]
public sealed class FmodGameAudioManager : ScriptableObject
{
    [SerializeField, FMODUnity.EventRef] private string SfxVolumeTestEvent;
    [SerializeField, FMODUnity.EventRef] private string UiVolumeTestEvent;

    private Bus MUSIC;
    private Bus UI_SFX;
    private Bus MASTER;
    private Bus GAME_SFX;
    private float MusicVolume = 0.5f;
    private float UI_SFXVolume = 0.5f;
    private float GameSFXVolume = 0.5f;
    private float MasterVolume = 1f;
    private EventInstance soundTest;
    private EventInstance uiTest;
    
    public bool IsInitialized { get; private set; }

    public void Init()
    {
        MASTER = FMODUnity.RuntimeManager.GetBus("bus:/MASTER");
        
        MUSIC = FMODUnity.RuntimeManager.GetBus("bus:/Master/MUSIC");
        MUSIC.setVolume(PlayerAudioPrefs.GetVolumeLevel(PlayerAudioPrefs.Music));
        
        UI_SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/UI_SFX");
        UI_SFX.setVolume(PlayerAudioPrefs.GetVolumeLevel(PlayerAudioPrefs.Ui));
        
        GAME_SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/GAME_SFX");
        GAME_SFX.setVolume(PlayerAudioPrefs.GetVolumeLevel(PlayerAudioPrefs.Sfx));
        
        soundTest = FMODUnity.RuntimeManager.CreateInstance(SfxVolumeTestEvent);
        uiTest = FMODUnity.RuntimeManager.CreateInstance(UiVolumeTestEvent);
        
        IsInitialized = true;
    }

    public void SetMasterVolumeLevel(float vol) => MasterVolumeLevel(vol);
    public void MasterVolumeLevel(float vol)
    {
        MasterVolume = vol;
        MASTER.setVolume(MasterVolume);
    }

    public void SetMusicVolumeLevel(float vol) => MusicVolumeLevel(vol);
    public void MusicVolumeLevel(float vol)
    {
        MusicVolume = vol;
        MUSIC.setVolume(MusicVolume);
    }
    
    public void SetGameSfxVolumeLevel(float vol) => GameSFXVolumeLevel(vol);
    public void GameSFXVolumeLevel(float vol)
    {
        GameSFXVolume = vol;
        GAME_SFX.setVolume(GameSFXVolume);

        soundTest.getPlaybackState(out var pbState);
        //if (pbState != PLAYBACK_STATE.PLAYING)
            soundTest.start();
    }
    
    public void SetUiSfxVolumeLevel(float vol) => SFXVolumeLevel(vol);
    public void SFXVolumeLevel(float vol)
    {
        UI_SFXVolume = vol;
        UI_SFX.setVolume(UI_SFXVolume);

        uiTest.getPlaybackState(out var pbState);
        //if (pbState != PLAYBACK_STATE.PLAYING)
            uiTest.start();
    }
}
