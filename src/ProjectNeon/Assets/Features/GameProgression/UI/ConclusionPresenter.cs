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
            victoryElements.ForEach(g => g.SetActive(true));
        else
            defeatElements.ForEach(g => g.SetActive(true));
    }

    private void HideElements()
    {
        victoryElements.ForEach(g => g.SetActive(false));
        defeatElements.ForEach(g => g.SetActive(false));
    }
}
