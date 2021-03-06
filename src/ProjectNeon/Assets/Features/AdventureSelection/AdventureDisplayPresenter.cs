﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdventureDisplayPresenter : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Button selectButton;
    
    [SerializeField] private Image[] heroBusts;
    [SerializeField] private GameObject[] heroIcons;
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
        allHeroesText.SetActive(adventure.RequiredHeroes.Length == 0);
        heroIcons[0].SetActive(adventure.RequiredHeroes.Length > 0);
        heroIcons[1].SetActive(adventure.RequiredHeroes.Length > 1);
        heroIcons[2].SetActive(adventure.RequiredHeroes.Length > 2);
        if (adventure.RequiredHeroes.Length > 0)
            heroBusts[0].sprite = adventure.RequiredHeroes[0].Bust;
        if (adventure.RequiredHeroes.Length > 1)
            heroBusts[1].sprite = adventure.RequiredHeroes[1].Bust;
        if (adventure.RequiredHeroes.Length > 2)
            heroBusts[2].sprite = adventure.RequiredHeroes[2].Bust;
    }
}