using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdventureDisplayPresenter : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Button selectButton;
    [SerializeField] private AdventureProgress adventureProgress;
    [SerializeField] private Navigator navigator;
    
    public void Init(Adventure adventure)
    {
        image.sprite = adventure.AdventureImage;
        nameText.text = adventure.Title;
        storyText.text = adventure.Story;
        selectButton.onClick.AddListener(() =>
        {
            adventureProgress.Init(adventure);
            navigator.NavigateToSquadSelection();
        });
    }
}