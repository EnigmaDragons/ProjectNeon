using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FmodMusicPlayer : MonoBehaviour
{
    private static EventInstance Music;
    private static EventInstance BattleLostStinger;
    private static EventInstance GameWonStinger;
    private static EventInstance CustomMusic;

    private void Awake()
    {
        Music.getPlaybackState(out var pbState);
        if (pbState == PLAYBACK_STATE.PLAYING) 
            return;
        
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic/GameMusic");
        Music.start();
        Music.release();
    }

    private void Start() => PlayMusicForScene(SceneManager.GetActiveScene().name);

    private void OnEnable()
    {
        Message.Subscribe<GameLostStingerMSG>(Battle_Lost_FUNC, this);
        Message.Subscribe<GameWonStingerMSG>(Battle_Won_FUNC, this);
        Message.Subscribe<NavigateToSceneRequested>(MusicChanger, this);
        Message.Subscribe<PlayCustomFmodMusic>(f => PlayCustomMusic(f.MusicName), this);
    }

    private void OnDisable() => Message.Unsubscribe(this);
    
    private const float TitleMusic = 0f;
    private const float GameMainMusic = 1f;
    private const float BattleMusic = 2f;
    private const float Nothing = 2f;
    private const float Med_Music = 5f;
    private const float ConclusionMusic = 3f;
    
    private readonly Dictionary<string, float> _musicProgressParamBySceneName = new Dictionary<string, float>
    {
        { "TitleScreen", TitleMusic },
        { "AdventureSelection", TitleMusic },
        { "AcademyScene", TitleMusic },
        { "SquadSelection", GameMainMusic },
        { "CutsceneScene", Med_Music },
        { "GameScene", GameMainMusic },
        { "GameSceneV4", GameMainMusic },
        { "AutoLoadGameScene", GameMainMusic },
        { "DeckBuilderTestScene", GameMainMusic },
        { "DeckBuilderScene", GameMainMusic },
        { "HoverVehicleScene", GameMainMusic },
        { "BattleSceneV2", BattleMusic },
        { "BattleTestScene", BattleMusic },
        { "ConclusionScene", ConclusionMusic }
    };

    private void PlayMusicForScene(string sceneName)
    {
        BattleLostStinger.stop(STOP_MODE.ALLOWFADEOUT);
        GameWonStinger.stop(STOP_MODE.ALLOWFADEOUT);
        CustomMusic.stop(STOP_MODE.ALLOWFADEOUT);
        
        var musicParam = _musicProgressParamBySceneName.ValueOrDefault(sceneName, () => GameMainMusic);
        Music.setParameterByName("MUSIC_PROGRESS", musicParam);
    }

    private void PlayCustomMusic(string musicName)
    {
        BattleLostStinger.stop(STOP_MODE.ALLOWFADEOUT);
        GameWonStinger.stop(STOP_MODE.ALLOWFADEOUT);
        CustomMusic.stop(STOP_MODE.ALLOWFADEOUT);
        
        Music.setParameterByName("MUSIC_PROGRESS", Nothing);
        if (CustomMusic.hasHandle())
            CustomMusic.release();
        
        CustomMusic = FMODUnity.RuntimeManager.CreateInstance(musicName);
        CustomMusic.start();
        CustomMusic.release();
    }

    private void MusicChanger(NavigateToSceneRequested msg) => PlayMusicForScene(msg.SceneName);
    
    private void Battle_Lost_FUNC(GameLostStingerMSG msg)
    {  
        BattleLostStinger = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic/Battle_Lost_Stinger");
        BattleLostStinger.start();
        BattleLostStinger.release();
    }
    
    private void Battle_Won_FUNC(GameWonStingerMSG msg)
    {
        GameWonStinger = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic/Game_Won_Stinger");
        GameWonStinger.start();
        GameWonStinger.release();
    }
}
