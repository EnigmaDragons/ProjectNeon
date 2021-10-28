using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FmodMusicPlayer : MonoBehaviour
{
    private static FMOD.Studio.EventInstance Music;
    private static FMOD.Studio.EventInstance BattleLostStinger;
    private FMOD.Studio.PLAYBACK_STATE PbState;

    private void Awake()
    {
       Music.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            Music = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic/GameMusic");
            Music.start();
            Music.release();
        }
    }
    private void Start()
    {

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "TitleScreen")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 0f);
        }
        if (sceneName == "AdventureSelection")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 0f);
        }
        if (sceneName == "SquadSelection")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 0f);
        }
        if (sceneName == "GameScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 1f);
        }
        if (sceneName == "AutoLoadGameScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 1f);
        }
        if (sceneName == "DeckBuilderTestScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 1f);
        }
        if (sceneName == "DeckBuilderScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 1f);
        }
        if (sceneName == "BattleSceneV2")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 2f);
        }
        if (sceneName == "BattleTestScene")
        {
            //FindObjectOfType<BattleTestSetup>().
            Music.setParameterByName("MUSIC_PROGRESS", 2f);
        }
        if (sceneName == "ConclusionScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 3f);
        }

    }

    private void OnEnable()
    {
        
        // Message.Subscribe<AdventureConclusionState>(Conclusion_Music, this);
        Message.Subscribe<BattleFinished>(Battle_Lost_FUNC, this);
        Message.Subscribe<NavigateToSceneRequested>(Battle_Lost_STOP_FUNC, this);
        Message.Subscribe<NavigateToSceneRequested>(MusicChanger, this);

    }

    private void MusicChanger(NavigateToSceneRequested msg)
    {
        {
            Music.setParameterByName("MUSIC_PROGRESS", 0f);
        }
        if (msg.SceneName == "SquadSelection")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 0f);
        }
        if (msg.SceneName == "GameScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 1f);
        }
        if (msg.SceneName == "AutoLoadGameScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 1f);
        }
        if (msg.SceneName == "DeckBuilderScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 1f);
        }
        if (msg.SceneName == "BattleSceneV2")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 2f);
        }
        if (msg.SceneName == "BattleTestScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 2f);
        }
        if (msg.SceneName == "ConclusionScene")
        {
            Music.setParameterByName("MUSIC_PROGRESS", 3f);
        }
    }

    private void Battle_Lost_FUNC(BattleFinished msg)
    {
        if (msg.Winner == TeamType.Enemies)
        {
            
            BattleLostStinger = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic/Battle_Lost_Stinger");
            BattleLostStinger.start();
            BattleLostStinger.release();
        }
    }
    private void Battle_Lost_STOP_FUNC(NavigateToSceneRequested msg)
    {
        if (msg.SceneName == "TitleScreen")
        BattleLostStinger.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    /*private void Conclusion_Music(AdventureConclusionState msg)
    {

        if (msg.IsVictorious)
        {
            Music.setParameterByName("MUSIC_PROGRESS", 4f);
        }

        else
        {
            Music.setParameterByName("MUSIC_PROGRESS", 3f);
        }
    }*/
    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

}
