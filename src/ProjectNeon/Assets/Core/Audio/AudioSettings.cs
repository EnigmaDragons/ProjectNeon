using UnityEngine;

public class AudioSettings : MonoBehaviour
{
     
    FMOD.Studio.EventInstance SFXVolumeTestEvent;

    FMOD.Studio.Bus MUSIC;
    FMOD.Studio.Bus UI_SFX;
    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus GameSFX;
    float MusicVolume = 0.5f;
    float UI_SFXVolume = 0.5f;
    float GameSFXVolume = 0.5f;
    float MasterVolume = 1f;

    void Awake()
    {
        MUSIC = FMODUnity.RuntimeManager.GetBus("bus:/Master/MUSIC");
        UI_SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/UI_SFX");
        Master = FMODUnity.RuntimeManager.GetBus("bus:/MASTER");
        GameSFX = FMODUnity.RuntimeManager.GetBus("bus:/ GameSFX");
        SFXVolumeTestEvent = FMODUnity.RuntimeManager.CreateInstance("event:/DeckBulder_Scene/Scene_Start");
    }

    void Update()
    {
        MUSIC.setVolume(MusicVolume);
        UI_SFX.setVolume(UI_SFXVolume);
        Master.setVolume(MasterVolume);
        GameSFX.setVolume(GameSFXVolume);
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
    }

    public void MusicVolumeLevel(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
    }

    public void SFXVolumeLevel(float newSFXVolume)
    {
        UI_SFXVolume = newSFXVolume;

        FMOD.Studio.PLAYBACK_STATE PbState;
        SFXVolumeTestEvent.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            SFXVolumeTestEvent.start();
        }
    }
}