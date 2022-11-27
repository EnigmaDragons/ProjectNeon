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
        Message.Subscribe<StopStandardFmodMusic>(_ => StopStandardMusic(), this);
    }

    private void OnDisable() => Message.Unsubscribe(this);
    
    private const float TitleMusic = 0f;
    private const float GameMainMusic = 1f;
    private const float BattleMusic = 2f;
    private const float Nothing = 2f;
    private const float Med_Music = 5f;
    private const float ConclusionMusic = 3f;
    private const float BattleWonStinger = 7f;

    private readonly Dictionary<string, float> _musicProgressParamBySceneName = new Dictionary<string, float>
    {
        { "TitleScreen", TitleMusic },
        { "AdventureSelection", TitleMusic },
        { "DraftAdventureSelection", TitleMusic },
        { "SettingsScene", TitleMusic },
        { "SquadSelection", TitleMusic },
        { "DatabaseScene", GameMainMusic },
        { "CutsceneScene", Med_Music },
        { "GameScene", GameMainMusic },
        { "GameSceneV4", GameMainMusic },
        { "AutoLoadGameScene", GameMainMusic },
        { "DeckBuilderTestScene", GameMainMusic },
        { "DeckBuilderScene", GameMainMusic },
        { "HoverVehicleScene", GameMainMusic },
        { "ConclusionScene", ConclusionMusic },
        { "AcademyScene", Nothing },
        { "BattleSceneV2", Nothing },
        { "BattleTestScene", Nothing },
        { "WishlistScene", Nothing },
    };

    private void PlayMusicForScene(string sceneName)
    {
        BattleLostStinger.stop(STOP_MODE.ALLOWFADEOUT);
        GameWonStinger.stop(STOP_MODE.ALLOWFADEOUT);
        CustomMusic.stop(STOP_MODE.ALLOWFADEOUT);
        
        var musicParam = _musicProgressParamBySceneName.ValueOrDefault(sceneName, () => GameMainMusic);
        Music.setParameterByName("MUSIC_PROGRESS", musicParam);
        
        if (sceneName == "AcademyScene")
            PlayCustomMusic("event:/GameMusic/ImmigrationOffice_Music");
    }

    private void PlayCustomMusic(string musicName)
    {
        Log.Info("Play Custom Music");
        StopStandardMusic();

        CustomMusic = FMODUnity.RuntimeManager.CreateInstance(musicName);
        CustomMusic.start();
        CustomMusic.release();
    }

    private static void StopStandardMusic()
    {
        BattleLostStinger.stop(STOP_MODE.ALLOWFADEOUT);
        GameWonStinger.stop(STOP_MODE.ALLOWFADEOUT);
        CustomMusic.stop(STOP_MODE.ALLOWFADEOUT);

        Music.setParameterByName("MUSIC_PROGRESS", Nothing);
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
