using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour
{

    public void NavigateToTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void NavigateToSquadSelection()
    {
        SceneManager.LoadScene("SquadSelection");
    }

    public void NavigateToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void NavigateToDeckBuilderScene()
    {
        SceneManager.LoadScene("DeckBuilderScene");
    }

    public void NavigateToBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }

    public void NavigateToVictoryScene()
    {
        SceneManager.LoadScene("VictoryScene");
    }

    public void NavigateToDefeatScene()
    {
        SceneManager.LoadScene("DefeatScene");
    }

}
