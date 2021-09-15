using UnityEngine;

[CreateAssetMenu(menuName = "Audio/FmodGameAudioManager")]
public sealed class FmodGameAudioManager : ScriptableObject
{
    FMOD.Studio.EventInstance SFXVolumeTestEvent;

    FMOD.Studio.Bus MUSIC;
    FMOD.Studio.Bus UI_SFX;
    FMOD.Studio.Bus MASTER;
    FMOD.Studio.Bus GameSFX;
    float MusicVolume = 0.5f;
    float UI_SFXVolume = 0.5f;
    float GameSFXVolume = 0.5f;
    float MasterVolume = 1f;

    public void Init()
    {
        MUSIC = FMODUnity.RuntimeManager.GetBus("bus:/Master/MUSIC");
        UI_SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/UI_SFX");
        MASTER = FMODUnity.RuntimeManager.GetBus("bus:/MASTER");
        GameSFX = FMODUnity.RuntimeManager.GetBus("bus:/GameSFX");
        SFXVolumeTestEvent = FMODUnity.RuntimeManager.CreateInstance("event:/DeckBulder_Scene/Aux_Click");
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
        MASTER.setVolume(MasterVolume);
    }

    public void MusicVolumeLevel(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
        MUSIC.setVolume(MusicVolume);
    }
    public void GameSFXVolumeLevel(float newGameSFXVolume)
    {
        GameSFXVolume = newGameSFXVolume;
        GameSFX.setVolume(GameSFXVolume);

    }
    public void SFXVolumeLevel(float newSFXVolume)
    {
        UI_SFXVolume = newSFXVolume;
        UI_SFX.setVolume(UI_SFXVolume);

        FMOD.Studio.PLAYBACK_STATE PbState;
        SFXVolumeTestEvent.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            SFXVolumeTestEvent.start();
        }
    }
}
