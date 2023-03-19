using I2.Loc;
using UnityEngine;

public class ConclusionPresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private Localize outcomeLabel;
    [SerializeField] private Localize adventureTitleLabel;
    [SerializeField] private Localize storyTextBox;
    [SerializeField] private GameObject[] victoryElements;
    [SerializeField] private GameObject[] defeatElements;

    private const string GameOverTerm = "Menu/Game Over";
    private const string WonTerm = "Menu/Won";
    
    public void Init(bool won, string adventureTitleTerm, string storyTextTerm)
    {
        outcomeLabel.SetTerm(won ? WonTerm : GameOverTerm);
        adventureTitleLabel.SetTerm(adventureTitleTerm);
        storyTextBox.SetTerm(storyTextTerm);
        HideElements();
        if (won)
        {
            if (victoryElements != null)
                victoryElements.ForEach(g => g.SetActive(true));
            Message.Publish(new GameWonStingerMSG(transform));
        }
        else
        {
            if (defeatElements != null)
                defeatElements.ForEach(g => g.SetActive(true));
            Message.Publish(new GameLostStingerMSG(transform));
        }
    }

    private void HideElements()
    {
        if (victoryElements != null)
            victoryElements.ForEach(g => g.SetActive(false));
        if (defeatElements != null)
            defeatElements.ForEach(g => g.SetActive(false));
    }

    public string[] GetLocalizeTerms()
        => new[] {WonTerm, GameOverTerm};
}
