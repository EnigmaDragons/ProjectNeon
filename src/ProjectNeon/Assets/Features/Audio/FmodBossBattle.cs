using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodBossBattle : MonoBehaviour
{
    private static FMOD.Studio.EventInstance BossMusic;
    private FMOD.Studio.PLAYBACK_STATE PbStateBoss;

    private void Start()
    {
        BossMusic.getPlaybackState(out PbStateBoss);
        if (PbStateBoss != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            BossMusic = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic/BossBattle");
            BossMusic.start();
            BossMusic.release();
        }
    }
    void OnEnable()
    {
        Message.Subscribe<NavigateToSceneRequested>(Music_StopperBoss, this);
        Message.Subscribe<WinBattleWithRewards>(OnStingerStopper, this);
    }

    private void Music_StopperBoss(NavigateToSceneRequested msg)
    {
        BossMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    private void OnStingerStopper(WinBattleWithRewards msg)
    {
        BossMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
}