using TMPro;
using UnityEngine;

public class ConclusionPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI outcomeLabel;
    [SerializeField] private TextMeshProUGUI adventureTitleLabel;
    [SerializeField] private TextMeshProUGUI storyTextBox;
    [SerializeField] private GameObject[] victoryElements;
    [SerializeField] private GameObject[] defeatElements;

    public void Init(bool won, string adventureTitle, string storyText)
    {
        outcomeLabel.text = won ? "You Won!" : "Game Over";
        adventureTitleLabel.text = adventureTitle;
        storyTextBox.text = storyText;
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
