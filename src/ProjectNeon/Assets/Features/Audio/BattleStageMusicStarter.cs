using System;
using Features.GameProgression;
using UnityEngine;

[Obsolete("No Longer Needed Due To FmodMusicPlayer.cs")]
public class BattleStageMusicStarter : MonoBehaviour
{
    [SerializeField] private GameMusicPlayer player;
    [SerializeField] private CurrentAdventureProgress adventure;
    [SerializeField] private AudioClipVolume defaultBattleTheme;

    private void Start()
    {
        var stageMusic = adventure.AdventureProgress.Stage.StageBattleTheme;
        var musicToPlay = stageMusic ?? defaultBattleTheme;
        player.PlaySelectedMusicLooping(musicToPlay.clip);
    }
}