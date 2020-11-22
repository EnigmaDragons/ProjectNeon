using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Navigator : ScriptableObject
{
    [SerializeField] private bool loggingEnabled;
    
    public void NavigateToTitleScreen() => NavigateTo("TitleScreen");
    public void NavigateToAdventureSelection() => NavigateTo("AdventureSelection");
    public void NavigateToSquadSelection() => NavigateTo("SquadSelection");
    public void NavigateToGameScene() => NavigateTo("GameScene");
    public void NavigateToGameScene2() => NavigateTo("GameScene2");
    public void NavigateToDeckBuilderScene() => NavigateTo("DeckBuilderScene");
    public void NavigateToBattleScene() => NavigateTo("BattleSceneV2");
    public void NavigateToVictoryScene() => NavigateTo("VictoryScene");
    public void NavigateToDefeatScene() => NavigateTo("DefeatScene");
    public void NavigateToShopScene() => NavigateTo("ShopScene");
    public void NavigateToRewardScene() => NavigateTo("RewardScene");

    private void NavigateTo(string name)
    {
        if (loggingEnabled)
            Log.Info($"Navigating to {name}");
        SceneManager.LoadScene(name);
    }
}
