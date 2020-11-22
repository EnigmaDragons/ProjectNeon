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
    
    [SerializeField] private Image heroBust;
    [SerializeField] private GameObject heroIcon1;
    [SerializeField] private GameObject allHeroesText;
    [SerializeField] private TextMeshProUGUI lengthText;
    
    public void Init(Adventure adventure, Action onSelect)
    {
        image.sprite = adventure.AdventureImage;
        nameText.text = adventure.Title;
        storyText.text = adventure.Story;
        lengthText.text = adventure.IsV2 ? adventure.DynamicStages.Length + " Chapters" : adventure.Stages.Length + " Chapters";
        DisplayHeroPool(adventure);
        selectButton.onClick.AddListener(() => onSelect());
    }

    private void DisplayHeroPool(Adventure adventure)
    {
        allHeroesText.SetActive(false);
        heroIcon1.SetActive(false);
        if (adventure.RequiredHeroes.Length > 0)
        {
            heroBust.sprite = adventure.RequiredHeroes[0].Bust;
            heroIcon1.SetActive(true);
        }
        else
        {
            allHeroesText.SetActive(true);
        }
    }
}