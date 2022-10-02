using UnityEngine;

public sealed class Navigator : ScriptableObject
{
    [SerializeField] private bool loggingEnabled;
    
    public void NavigateToTitleScreen() => NavigateTo("TitleScreen");
    public void NavigateToAdventureSelection() => NavigateTo("AdventureSelection");
    public void NavigateToDraftAdventureSelection() => NavigateTo("DraftAdventureSelection");
    public void NavigateToSquadSelection() => NavigateTo("SquadSelection");
    public void NavigateToGameScene() => NavigateTo("GameScene");
    public void NavigateToGameSceneV4() => NavigateTo("GameSceneV4");
    public void NavigateToGameSceneV5() => NavigateTo("GameSceneV5");
    public void NavigateToDeckBuilderScene() => NavigateTo("DeckBuilderScene");
    public void NavigateToBattleScene() => NavigateTo("BattleSceneV2");
    public void NavigateToConclusionScene() => NavigateTo("ConclusionScene");
    public void NavigateToShopScene() => NavigateTo("ShopScene");
    public void NavigateToRewardScene() => NavigateTo("RewardScene");
    public void NavigateToCutsceneScene() => NavigateTo("CutsceneScene");
    public void NavigateToAcademyScene() => NavigateTo("AcademyScene");
    public void NavigateToDatabaseScene() => NavigateTo("DatabaseScene");
    public void NavigateToDiscordServer() => Application.OpenURL("https://discord.gg/V3yKWAwknC");
    public void NavigateToSteamPage() => Application.OpenURL("https://store.steampowered.com/app/1412960/Metroplex_Zero/");
    public void NavigateToContentCreatorsPage() => Application.OpenURL("https://metroplexzero.com/index.html?page=contentcreators");
    public void NavigateToDemoSurvey() => Application.OpenURL("https://forms.gle/SsgkqR4D7GDLmS4Y9");
    public void NavigateToSettingsScene() => NavigateTo("SettingsScene");
    public void NavigateToDifficultyScene() => NavigateTo("DifficultySelection");
    public void NavigateToWishlistScene() => NavigateTo("WishlistScene");

    public void ReloadScene()
    {
        if (loggingEnabled)
            Log.Info($"Reloading scene");
        Message.Publish(new ReloadSceneRequested());
    }

    private void NavigateTo(string sceneName)
    {
        if (loggingEnabled)
            Log.Info($"Navigating to {sceneName}");
        Message.Publish(new NavigateToSceneRequested(sceneName));
    }
    
    public void ExitGame()
    {     
        Message.Publish(new ExitGameRequested());
    }

    public static void HardExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
#else
        Application.Quit();
#endif 
    }
}
