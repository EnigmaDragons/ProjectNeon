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
        if(msg.CurrentSceneName == "AdventureSelection")
            Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnDestroy()
    {
        Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
