﻿using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Navigator : ScriptableObject
{
    [SerializeField] private bool loggingEnabled;
    
    public void NavigateToTitleScreen() => NavigateTo("TitleScreen");
    public void NavigateToSquadSelection() => NavigateTo("SquadSelection");
    public void NavigateToGameScene() => NavigateTo("GameScene");
    public void NavigateToDeckBuilderScene() => NavigateTo("DeckBuilderScene");
    public void NavigateToBattleScene() => NavigateTo("BattleScene");
    public void NavigateToVictoryScene() => NavigateTo("VictoryScene");
    public void NavigateToDefeatScene() => NavigateTo("DefeatScene");
    public void NavigateToShopScene() => NavigateTo("ShopScene");
    public void NavigateToRewardScene() => NavigateTo("RewardScene");

    private void NavigateTo(string name)
    {
        if (loggingEnabled)
            Debug.Log($"Navigating to {name}");
        SceneManager.LoadScene(name);
    }
}
