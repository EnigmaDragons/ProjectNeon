using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodMusicPlayer : MonoBehaviour
{
    private static FMOD.Studio.EventInstance Music;

    private void Start()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic/GameMusic");
        Music.start();
        Music.release();
    }

    private void OnEnable()
    {
        Message.Subscribe<SceneChanged>(Music_Changer, this);
    }

    private void Music_Changer(SceneChanged msg)
    {
        if(msg.CurrentSceneName == "TitleScreen")
            Music.setParameterByName("MUSIC_PROGRESS", 0f);
        if (msg.CurrentSceneName == "AdventureSelection")
            Music.setParameterByName("MUSIC_PROGRESS", 0f);
        if (msg.CurrentSceneName == "SquadSelection")
            Music.setParameterByName("MUSIC_PROGRESS", 0f);
        if (msg.CurrentSceneName == "GameScene")
            Music.setParameterByName("MUSIC_PROGRESS", 1f);
        if (msg.CurrentSceneName == "DeckBuilderScene")
            Music.setParameterByName("MUSIC_PROGRESS", 1f);
        if (msg.CurrentSceneName == "BattleSceneV2")
            Music.setParameterByName("MUSIC_PROGRESS", 2f);
    }

    private void OnDestroy()
    {
        Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
