using System;
using UnityEngine;

public class FmodZantoBattleMusicPlayer : MonoBehaviour
{
    private static FMOD.Studio.EventInstance BattleMusic;
    private FMOD.Studio.PLAYBACK_STATE PbState;

    private void Start()
    {
        BattleMusic.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            BattleMusic = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic/ZantoBattle");
            BattleMusic.start();
            BattleMusic.release();
        }
    }
    void OnEnable()
    {
        Message.Subscribe<NavigateToSceneRequested>(Music_Stopper, this);
        Message.Subscribe<WinBattleWithRewards>(OnStingerStopper, this);
    }

    private void OnStingerStopper(WinBattleWithRewards msg)
    {
        BattleMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void Music_Stopper(NavigateToSceneRequested msg)
    {
        BattleMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
}
