using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdventureDisplayPresenter : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Button selectButton;
    
    public void Init(Adventure adventure, Action onSelect)
    {
        image.sprite = adventure.AdventureImage;
        nameText.text = adventure.Title;
        storyText.text = adventure.Story;
        selectButton.onClick.AddListener(() => onSelect());
    }
}