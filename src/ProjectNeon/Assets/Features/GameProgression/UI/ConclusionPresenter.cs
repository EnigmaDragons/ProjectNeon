using I2.Loc;
using TMPro;
using UnityEngine;

public class ConclusionPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI outcomeLabel;
    [SerializeField] private Localize adventureTitleLabel;
    [SerializeField] private Localize storyTextBox;
    [SerializeField] private GameObject[] victoryElements;
    [SerializeField] private GameObject[] defeatElements;

    public void Init(bool won, string adventureTitleTerm, string storyTextTerm)
    {
        outcomeLabel.text = won ? "You Won!" : "Game Over";
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
}
