using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Navigator : ScriptableObject
{
    [SerializeField] private bool loggingEnabled;
    
    public void NavigateToTitleScreen() => NavigateTo("TitleScreen");
    public void NavigateToAdventureSelection() => NavigateTo("AdventureSelection");
    public void NavigateToSquadSelection() => NavigateTo("SquadSelection");
    public void NavigateToGameScene() => NavigateTo("GameScene");
    public void NavigateToGameSceneV4() => NavigateTo("GameSceneV4");
    public void NavigateToDeckBuilderScene() => NavigateTo("DeckBuilderScene");
    public void NavigateToBattleScene() => NavigateTo("BattleSceneV2");
    public void NavigateToConclusionScene() => NavigateTo("ConclusionScene");
    public void NavigateToShopScene() => NavigateTo("ShopScene");
    public void NavigateToRewardScene() => NavigateTo("RewardScene");
    public void NavigateToCutsceneScene() => NavigateTo("CutsceneScene");
    public void NavigateToDiscordServer() => Application.OpenURL("https://discord.gg/V3yKWAwknC");
    public void NavigateToSteamPage() => Application.OpenURL("https://store.steampowered.com/app/1412960/Metroplex_Zero/");
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
#else
        Application.Quit();
#endif
    }
}
