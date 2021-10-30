using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodMainBattleMusicPlayer : MonoBehaviour
{
    private static FMOD.Studio.EventInstance BattleMusic;
    private FMOD.Studio.PLAYBACK_STATE PbState;

    private void Start()
    {
        BattleMusic.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            BattleMusic = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic/BattleMusic1");
            BattleMusic.start();
            BattleMusic.release();
        }
    }
    void OnEnable()
    {
        Message.Subscribe<NavigateToSceneRequested>(Music_Stopper, this);
    }

    private void Music_Stopper(NavigateToSceneRequested msg)
    {
        if (msg.SceneName == "GameScene")
        {
            BattleMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        if (msg.SceneName == "ConclusionScene")
        {
            BattleMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        if (msg.SceneName == "TitleScreen")
        {
            BattleMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
}
