using System;
using UnityEngine;

[Obsolete("No Longer Needed Due To FmodMusicPlayer.cs")]
public class BattleStageMusicStarter : MonoBehaviour
{
    [SerializeField] private GameMusicPlayer player;
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private AudioClipVolume defaultBattleTheme;

    private void Start()
    {
        var stageMusic = adventure.CurrentChapter.StageBattleTheme;
        var musicToPlay = stageMusic ?? defaultBattleTheme;
        player.PlaySelectedMusicLooping(musicToPlay.clip);
    }
}